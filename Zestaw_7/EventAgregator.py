__author__ = 'Mira'

import uuid
import sys
from PyQt4 import QtGui
from PyQt4.QtCore import Qt


class Element(QtGui.QTreeWidgetItem):
    def __init__(self, collection, *args):
        super().__init__(*args)
        self.id = uuid.uuid4()
        self.collection = collection


class Singleton(type):
    _instance = None

    def __call__(cls, *args, **kwargs):
        if not cls._instance:
            cls._instance = super().__call__(*args, **kwargs)
        return cls._instance


class Aggregator(object):
    __metaclass__ = Singleton
    _listeners = dict()

    def subscribe(self, name, callback):
        Aggregator._listeners.setdefault(name, []).append(callback)

    def unsubscribe(self, name, callback):
        try:
            Aggregator._listeners[name].remove(callback)
            if len(Aggregator._listeners[name]) == 0:
                del Aggregator._listeners[name]
        except (KeyError, ValueError):
            pass

    def publish(self, name, *args, **kwargs):
        for callback in Aggregator._listeners.get(name) or []:
            callback(*args, **kwargs)


class Dialog(QtGui.QDialog):
    def __init__(self, collection, id=None, parent=None):
        super().__init__(parent)
        self.collection = collection
        self.id = id
        self.name = QtGui.QLineEdit(self)
        self.name.textEdited.connect(self.editHandler)
        self.surname = QtGui.QLineEdit(self)
        self.surname.textEdited.connect(self.editHandler)
        self.birthDate = QtGui.QLineEdit(self)
        self.birthDate.textEdited.connect(self.editHandler)
        self.address = QtGui.QLineEdit(self)
        self.address.textEdited.connect(self.editHandler)
        layout = QtGui.QFormLayout()
        layout.addRow("Name:", self.name)
        layout.addRow("Surname:", self.surname)
        layout.addRow("Birth date:", self.birthDate)
        layout.addRow("Address:", self.address)
        self.setLayout(layout)

        if self.id is None:
            Aggregator().subscribe(
                'list/{}Added'.format(self.collection), self.setId
            )

    def setId(self, id, _):
        self.id = id
        Aggregator().unsubscribe(
            'list/{}Added'.format(self.collection), self.setId
        )

    def setValues(self, surname, name, birthDate, address):
        self.name.setText(surname)
        self.surname.setText(name)
        self.birthDate.setText(birthDate)
        self.address.setText(address)
        return self

    def editHandler(self, _):
        if self.id is None:
            Aggregator().publish(
                'dialog/{}Added'.format(self.collection),
                self.name.text(),
                self.surname.text(),
                self.birthDate.text(),
                self.address.text()
            )
        else:
            Aggregator().publish(
                'dialog/{}Edited'.format(self.collection),
                self.id,
                self.name.text(),
                self.surname.text(),
                self.birthDate.text(),
                self.address.text()
            )


class Tree(QtGui.QTreeWidget):
    def __init__(self):
        super().__init__()
        self.setColumnCount(1)
        self.lecturers = Element('Lecturer', ["Wykładowcy"])
        self.students = Element('Student', ["Studenci"])
        self.addTopLevelItems([self.lecturers, self.students])
        self.setHeaderHidden(True)
        self.itemClicked.connect(self.clickHandler)

        Aggregator().subscribe("list/LecturerAdded", self.addLecturer)
        Aggregator().subscribe("list/StudentAdded", self.addStudent)
        Aggregator().subscribe("list/ElementEdited", self.editElement)

    def addLecturer(self, id, name):
        item = QtGui.QTreeWidgetItem([name])
        item.id = id
        self.lecturers.addChild(item)

    def addStudent(self, id, name):
        item = QtGui.QTreeWidgetItem([name])
        item.id = id
        self.students.addChild(item)

    def editElement(self, id, name):
        for i in range(self.topLevelItemCount()):
            item = self.topLevelItem(i)
            for j in range(item.childCount()):
                sitem = item.child(j)
                if sitem.id == id:
                    sitem.setText(0, name)

    def clickHandler(self, item, column):
        p = item.parent()
        Aggregator().publish("tree/StateChanged", p and 'edit' or 'add')
        Aggregator().publish(
            "tree/SelectionChanged", p and item.id or item.collection
        )


class List(QtGui.QTreeWidget):
    def __init__(self):
        super().__init__()
        self.setColumnCount(4)
        self.setHeaderLabels(["Nazwisko", "Imię", "Data urodzenia", "Adres"])
        self.setIndentation(0)
        self.state = None
        self.state2 = None
        self.itemDoubleClicked.connect(self.doubleClickHandler)

        Aggregator().subscribe("tree/StateChanged", self.setState)
        Aggregator().subscribe("tree/SelectionChanged", self.setElements)
        Aggregator().subscribe("dialog/LecturerAdded", self.addLecturer)
        Aggregator().subscribe("dialog/StudentAdded", self.addStudent)
        Aggregator().subscribe("dialog/LecturerEdited", self.editElement)
        Aggregator().subscribe("dialog/StudentEdited", self.editElement)

    def addLecturer(self, name, surname, birthDate, address):
        e = Element('Lecturer', [surname, name, birthDate, address])
        self.addTopLevelItem(e)

        Aggregator().publish(
            "list/LecturerAdded", e.id, "{} {}".format(name, surname)
        )

    def addStudent(self, name, surname, birthDate, address):
        e = Element('Student', [surname, name, birthDate, address])
        self.addTopLevelItem(e)

        Aggregator().publish(
            "list/StudentAdded", e.id, "{} {}".format(name, surname)
        )

    def editElement(self, id, name, surname, birthDate, address):
        for i in range(self.topLevelItemCount()):
            item = self.topLevelItem(i)
            if item.id == id:
                item.setText(0, surname)
                item.setText(1, name)
                item.setText(2, birthDate)
                item.setText(3, address)

        Aggregator().publish(
            'list/ElementEdited', id, "{} {}".format(name, surname)
        )

    def setElements(self, id):
        for i in range(self.topLevelItemCount()):
            item = self.topLevelItem(i)
            item.setHidden(id != item.id and id != item.collection)

    def setState(self, state):
        self.state = state

    def doubleClickHandler(self, item, column):
        if self.state == 'add':
            Dialog(self.topLevelItem(0).collection).exec_()
        elif self.state == 'edit':
            Dialog(self.topLevelItem(0).collection, item.id).setValues(
                item.text(1), item.text(0), item.text(2), item.text(3)
            ).exec_()


class Main(QtGui.QMainWindow):
    def __init__(self):
        super().__init__()
        self.tree = Tree()
        self.list_ = List()

        self.list_.addLecturer(
            "Krzysztof", "Silor", "1960-01-02", "Kwiatowa 2/14, Wrocław"
        )
        self.list_.addLecturer(
            "Jerzy", "Tomaszewski", "1973-12-30", "Śliwowa 1/104, Wrocław"
        )
        self.list_.addStudent(
            "Jan", "Kochanowski", "1989-09-02", "Fabryczna 26/3, Wrocław"
        )
        self.list_.addStudent(
            "Tomasz", "Makowski", "1990-03-20", "Miodowa 2/12, Wrocław"
        )

        splitter = QtGui.QSplitter(Qt.Horizontal)
        splitter.addWidget(self.tree)
        splitter.addWidget(self.list_)
        self.setCentralWidget(splitter)


if __name__ == '__main__':
    app = QtGui.QApplication(sys.argv)
    app.setApplicationName('poolist7.zad2')
    main = Main()
    main.show()
    sys.exit(app.exec_())
