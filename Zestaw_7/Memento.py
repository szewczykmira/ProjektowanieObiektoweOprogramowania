import sys
from PyQt4 import QtGui
from PyQt4.QtCore import Qt, pyqtSignal


class ShapeCircle(QtGui.QGraphicsEllipseItem):
    def __init__(self, r):
        super().__init__(0, 0, r, r)
        self.setPen(QtGui.QPen(
            QtGui.QBrush(QtGui.QColor(10, 10, 90)), 3
        ))
        self.shape = 'C'

    def clone(self):
        return ShapeCircle(self.boundingRect().width())


class ShapeSquare(QtGui.QGraphicsRectItem):
    def __init__(self, r):
        super().__init__(0, 0, r, r)
        self.setPen(QtGui.QPen(
            QtGui.QBrush(QtGui.QColor(150, 50, 30)), 5
        ))
        self.shape = 'S'

    def clone(self):
        return ShapeSquare(self.boundingRect().width())


class ShapeRect(QtGui.QGraphicsRectItem):
    def __init__(self, w, h):
        super().__init__(0, 0, w, h)
        self.setPen(QtGui.QPen(
            QtGui.QBrush(QtGui.QColor(10, 255, 90)), 14,
            join=Qt.MiterJoin
        ))
        self.shape = 'R'

    def clone(self):
        size = self.boundingRect()
        return ShapeRect(size.width(), size.height())


class Memento(object):
    def __init__(self, action, shape, pos):
        self.action = action
        self.shape = shape
        self.pos = pos


class Scene(QtGui.QGraphicsScene):
    _circle = ShapeCircle(30)
    _square = ShapeSquare(50)
    _rect = ShapeRect(40, 60)

    def __init__(self, parent=None):
        super().__init__(parent)
        self.mode = ''
        self._item = None
        self._undoStack = list()
        self._redoStack = list()

    def undo(self):
        pass

    def redo(self):
        pass

    def setMode(self, mode):
        self.mode = mode

    def mousePressEvent(self, event):
        pos = event.scenePos()
        item = None
        if self.mode == 'C':
            item = Scene._circle.clone()
        elif self.mode == 'S':
            item = Scene._square.clone()
        elif self.mode == 'R':
            item = Scene._rect.clone()
        elif self.mode == 'M':
            self._item = self.itemAt(pos)
        elif self.mode == 'E':
            item = self.itemAt(pos)
            if item:
                #self._undoStack.append(Memento('rm', item.shape, pos))
                self.removeItem(item)
        if item:
            item.setPos(pos)
            self.addItem(item)
            self._undoStack.append(Memento('add', self.mode, pos))
        super().mousePressEvent(event)

    def mouseMoveEvent(self, event):
        if self._item:
            self._item.setPos(event.scenePos())
        super().mouseMoveEvent(event)

    def mouseReleaseEvent(self, event):
        self._item = None
        super().mouseReleaseEvent(event)


class Sketchboard(QtGui.QGraphicsView):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setScene(Scene(self))
        self.setAlignment(Qt.AlignLeft | Qt.AlignTop)
        self.setMouseTracking(True)


class Main(QtGui.QMainWindow):
    def __init__(self):
        super().__init__()
        self.board = Sketchboard()
        self.circle = QtGui.QPushButton("Circle")
        self.circle.clicked.connect(lambda: self.board.scene().setMode('C'))
        self.square = QtGui.QPushButton("Square")
        self.square.clicked.connect(lambda: self.board.scene().setMode('S'))
        self.rectangle = QtGui.QPushButton("Rectangle")
        self.rectangle.clicked.connect(lambda: self.board.scene().setMode('R'))
        self.move = QtGui.QPushButton("Move")
        self.move.clicked.connect(lambda: self.board.scene().setMode('M'))
        self.erase = QtGui.QPushButton("Erase")
        self.erase.clicked.connect(lambda: self.board.scene().setMode('E'))
        self.undo = QtGui.QPushButton("Undo")
        self.redo = QtGui.QPushButton("Redo")
        vlayout = QtGui.QHBoxLayout()
        vlayout.addWidget(self.circle)
        vlayout.addWidget(self.square)
        vlayout.addWidget(self.rectangle)
        vlayout.addWidget(self.move)
        vlayout.addWidget(self.erase)
        vlayout.addWidget(self.undo)
        vlayout.addWidget(self.redo)
        widget = QtGui.QWidget()
        hlayout = QtGui.QVBoxLayout()
        hlayout.addLayout(vlayout)
        hlayout.addWidget(self.board)
        widget.setLayout(hlayout)
        self.setCentralWidget(widget)


if __name__ == '__main__':
    app = QtGui.QApplication(sys.argv)
    app.setApplicationName('poolist7.zad3')
    main = Main()
    main.show()
    sys.exit(app.exec_())