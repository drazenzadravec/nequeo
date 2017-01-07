/****************************************************************************
** Meta object code from reading C++ file 'WebSocketServer.h'
**
** Created by: The Qt Meta Object Compiler version 67 (Qt 5.7.0)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "stdafx.h"

#include "WebSocketServer.h"
#include <QtCore/qbytearray.h>
#include <QtCore/qmetatype.h>
#include <QtCore/QList>
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'WebSocketServer.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 67
#error "This file was generated using the moc from 5.7.0. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
struct qt_meta_stringdata_Nequeo__Net__WebSocket__Server_t {
    QByteArrayData data[19];
    char stringdata0[353];
};
#define QT_MOC_LITERAL(idx, ofs, len) \
    Q_STATIC_BYTE_ARRAY_DATA_HEADER_INITIALIZER_WITH_OFFSET(len, \
    qptrdiff(offsetof(qt_meta_stringdata_Nequeo__Net__WebSocket__Server_t, stringdata0) + ofs \
        - idx * sizeof(QByteArrayData)) \
    )
static const qt_meta_stringdata_Nequeo__Net__WebSocket__Server_t qt_meta_stringdata_Nequeo__Net__WebSocket__Server = {
    {
QT_MOC_LITERAL(0, 0, 30), // "Nequeo::Net::WebSocket::Server"
QT_MOC_LITERAL(1, 31, 23), // "OnClientConnectedHandle"
QT_MOC_LITERAL(2, 55, 0), // ""
QT_MOC_LITERAL(3, 56, 26), // "OnClientDisconnectedHandle"
QT_MOC_LITERAL(4, 83, 17), // "OnSslErrorsHandle"
QT_MOC_LITERAL(5, 101, 16), // "QList<QSslError>"
QT_MOC_LITERAL(6, 118, 6), // "errors"
QT_MOC_LITERAL(7, 125, 25), // "OnClientSocketErrorHandle"
QT_MOC_LITERAL(8, 151, 28), // "QAbstractSocket::SocketError"
QT_MOC_LITERAL(9, 180, 5), // "error"
QT_MOC_LITERAL(10, 186, 27), // "OnTextMessageReceivedHandle"
QT_MOC_LITERAL(11, 214, 7), // "message"
QT_MOC_LITERAL(12, 222, 29), // "OnBinaryMessageReceivedHandle"
QT_MOC_LITERAL(13, 252, 18), // "OnClientPongHandle"
QT_MOC_LITERAL(14, 271, 11), // "elapsedTime"
QT_MOC_LITERAL(15, 283, 7), // "payload"
QT_MOC_LITERAL(16, 291, 26), // "OnClientStateChangedHandle"
QT_MOC_LITERAL(17, 318, 28), // "QAbstractSocket::SocketState"
QT_MOC_LITERAL(18, 347, 5) // "state"

    },
    "Nequeo::Net::WebSocket::Server\0"
    "OnClientConnectedHandle\0\0"
    "OnClientDisconnectedHandle\0OnSslErrorsHandle\0"
    "QList<QSslError>\0errors\0"
    "OnClientSocketErrorHandle\0"
    "QAbstractSocket::SocketError\0error\0"
    "OnTextMessageReceivedHandle\0message\0"
    "OnBinaryMessageReceivedHandle\0"
    "OnClientPongHandle\0elapsedTime\0payload\0"
    "OnClientStateChangedHandle\0"
    "QAbstractSocket::SocketState\0state"
};
#undef QT_MOC_LITERAL

static const uint qt_meta_data_Nequeo__Net__WebSocket__Server[] = {

 // content:
       7,       // revision
       0,       // classname
       0,    0, // classinfo
       8,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: name, argc, parameters, tag, flags
       1,    0,   54,    2, 0x08 /* Private */,
       3,    0,   55,    2, 0x08 /* Private */,
       4,    1,   56,    2, 0x08 /* Private */,
       7,    1,   59,    2, 0x08 /* Private */,
      10,    1,   62,    2, 0x08 /* Private */,
      12,    1,   65,    2, 0x08 /* Private */,
      13,    2,   68,    2, 0x08 /* Private */,
      16,    1,   73,    2, 0x08 /* Private */,

 // slots: parameters
    QMetaType::Void,
    QMetaType::Void,
    QMetaType::Void, 0x80000000 | 5,    6,
    QMetaType::Void, 0x80000000 | 8,    9,
    QMetaType::Void, QMetaType::QString,   11,
    QMetaType::Void, QMetaType::QByteArray,   11,
    QMetaType::Void, QMetaType::ULongLong, QMetaType::QByteArray,   14,   15,
    QMetaType::Void, 0x80000000 | 17,   18,

       0        // eod
};

void Nequeo::Net::WebSocket::Server::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Server *_t = static_cast<Server *>(_o);
        Q_UNUSED(_t)
        switch (_id) {
        case 0: _t->OnClientConnectedHandle(); break;
        case 1: _t->OnClientDisconnectedHandle(); break;
        case 2: _t->OnSslErrorsHandle((*reinterpret_cast< const QList<QSslError>(*)>(_a[1]))); break;
        case 3: _t->OnClientSocketErrorHandle((*reinterpret_cast< QAbstractSocket::SocketError(*)>(_a[1]))); break;
        case 4: _t->OnTextMessageReceivedHandle((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 5: _t->OnBinaryMessageReceivedHandle((*reinterpret_cast< QByteArray(*)>(_a[1]))); break;
        case 6: _t->OnClientPongHandle((*reinterpret_cast< quint64(*)>(_a[1])),(*reinterpret_cast< QByteArray(*)>(_a[2]))); break;
        case 7: _t->OnClientStateChangedHandle((*reinterpret_cast< QAbstractSocket::SocketState(*)>(_a[1]))); break;
        default: ;
        }
    }
}

const QMetaObject Nequeo::Net::WebSocket::Server::staticMetaObject = {
    { &QObject::staticMetaObject, qt_meta_stringdata_Nequeo__Net__WebSocket__Server.data,
      qt_meta_data_Nequeo__Net__WebSocket__Server,  qt_static_metacall, Q_NULLPTR, Q_NULLPTR}
};


const QMetaObject *Nequeo::Net::WebSocket::Server::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->dynamicMetaObject() : &staticMetaObject;
}

void *Nequeo::Net::WebSocket::Server::qt_metacast(const char *_clname)
{
    if (!_clname) return Q_NULLPTR;
    if (!strcmp(_clname, qt_meta_stringdata_Nequeo__Net__WebSocket__Server.stringdata0))
        return static_cast<void*>(const_cast< Server*>(this));
    return QObject::qt_metacast(_clname);
}

int Nequeo::Net::WebSocket::Server::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QObject::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 8)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 8;
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        if (_id < 8)
            *reinterpret_cast<int*>(_a[0]) = -1;
        _id -= 8;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
