/****************************************************************************
** Meta object code from reading C++ file 'WebSocketClient.h'
**
** Created by: The Qt Meta Object Compiler version 67 (Qt 5.7.0)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "stdafx.h"

#include "WebSocketClient.h"
#include <QtCore/qbytearray.h>
#include <QtCore/qmetatype.h>
#include <QtCore/QList>
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'WebSocketClient.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 67
#error "This file was generated using the moc from 5.7.0. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
struct qt_meta_stringdata_Nequeo__Net__WebSocket__Client_t {
    QByteArrayData data[30];
    char stringdata0[523];
};
#define QT_MOC_LITERAL(idx, ofs, len) \
    Q_STATIC_BYTE_ARRAY_DATA_HEADER_INITIALIZER_WITH_OFFSET(len, \
    qptrdiff(offsetof(qt_meta_stringdata_Nequeo__Net__WebSocket__Client_t, stringdata0) + ofs \
        - idx * sizeof(QByteArrayData)) \
    )
static const qt_meta_stringdata_Nequeo__Net__WebSocket__Client_t qt_meta_stringdata_Nequeo__Net__WebSocket__Client = {
    {
QT_MOC_LITERAL(0, 0, 30), // "Nequeo::Net::WebSocket::Client"
QT_MOC_LITERAL(1, 31, 17), // "OnConnectedHandle"
QT_MOC_LITERAL(2, 49, 0), // ""
QT_MOC_LITERAL(3, 50, 19), // "OnDisonnectedHandle"
QT_MOC_LITERAL(4, 70, 14), // "OnAboutToClose"
QT_MOC_LITERAL(5, 85, 27), // "OnTextMessageReceivedHandle"
QT_MOC_LITERAL(6, 113, 7), // "message"
QT_MOC_LITERAL(7, 121, 29), // "OnBinaryMessageReceivedHandle"
QT_MOC_LITERAL(8, 151, 17), // "OnSslErrorsHandle"
QT_MOC_LITERAL(9, 169, 16), // "QList<QSslError>"
QT_MOC_LITERAL(10, 186, 6), // "errors"
QT_MOC_LITERAL(11, 193, 19), // "OnSocketErrorHandle"
QT_MOC_LITERAL(12, 213, 28), // "QAbstractSocket::SocketError"
QT_MOC_LITERAL(13, 242, 5), // "error"
QT_MOC_LITERAL(14, 248, 12), // "OnPongHandle"
QT_MOC_LITERAL(15, 261, 11), // "elapsedTime"
QT_MOC_LITERAL(16, 273, 7), // "payload"
QT_MOC_LITERAL(17, 281, 20), // "OnStateChangedHandle"
QT_MOC_LITERAL(18, 302, 28), // "QAbstractSocket::SocketState"
QT_MOC_LITERAL(19, 331, 5), // "state"
QT_MOC_LITERAL(20, 337, 35), // "OnProxyAuthenticationRequired..."
QT_MOC_LITERAL(21, 373, 13), // "QNetworkProxy"
QT_MOC_LITERAL(22, 387, 5), // "proxy"
QT_MOC_LITERAL(23, 393, 15), // "QAuthenticator*"
QT_MOC_LITERAL(24, 409, 13), // "authenticator"
QT_MOC_LITERAL(25, 423, 27), // "OnReadChannelFinishedHandle"
QT_MOC_LITERAL(26, 451, 25), // "OnTextFrameReceivedHandle"
QT_MOC_LITERAL(27, 477, 5), // "frame"
QT_MOC_LITERAL(28, 483, 11), // "isLastFrame"
QT_MOC_LITERAL(29, 495, 27) // "OnBinaryFrameReceivedHandle"

    },
    "Nequeo::Net::WebSocket::Client\0"
    "OnConnectedHandle\0\0OnDisonnectedHandle\0"
    "OnAboutToClose\0OnTextMessageReceivedHandle\0"
    "message\0OnBinaryMessageReceivedHandle\0"
    "OnSslErrorsHandle\0QList<QSslError>\0"
    "errors\0OnSocketErrorHandle\0"
    "QAbstractSocket::SocketError\0error\0"
    "OnPongHandle\0elapsedTime\0payload\0"
    "OnStateChangedHandle\0QAbstractSocket::SocketState\0"
    "state\0OnProxyAuthenticationRequiredHandle\0"
    "QNetworkProxy\0proxy\0QAuthenticator*\0"
    "authenticator\0OnReadChannelFinishedHandle\0"
    "OnTextFrameReceivedHandle\0frame\0"
    "isLastFrame\0OnBinaryFrameReceivedHandle"
};
#undef QT_MOC_LITERAL

static const uint qt_meta_data_Nequeo__Net__WebSocket__Client[] = {

 // content:
       7,       // revision
       0,       // classname
       0,    0, // classinfo
      13,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: name, argc, parameters, tag, flags
       1,    0,   79,    2, 0x08 /* Private */,
       3,    0,   80,    2, 0x08 /* Private */,
       4,    0,   81,    2, 0x08 /* Private */,
       5,    1,   82,    2, 0x08 /* Private */,
       7,    1,   85,    2, 0x08 /* Private */,
       8,    1,   88,    2, 0x08 /* Private */,
      11,    1,   91,    2, 0x08 /* Private */,
      14,    2,   94,    2, 0x08 /* Private */,
      17,    1,   99,    2, 0x08 /* Private */,
      20,    2,  102,    2, 0x08 /* Private */,
      25,    0,  107,    2, 0x08 /* Private */,
      26,    2,  108,    2, 0x08 /* Private */,
      29,    2,  113,    2, 0x08 /* Private */,

 // slots: parameters
    QMetaType::Void,
    QMetaType::Void,
    QMetaType::Void,
    QMetaType::Void, QMetaType::QString,    6,
    QMetaType::Void, QMetaType::QByteArray,    6,
    QMetaType::Void, 0x80000000 | 9,   10,
    QMetaType::Void, 0x80000000 | 12,   13,
    QMetaType::Void, QMetaType::ULongLong, QMetaType::QByteArray,   15,   16,
    QMetaType::Void, 0x80000000 | 18,   19,
    QMetaType::Void, 0x80000000 | 21, 0x80000000 | 23,   22,   24,
    QMetaType::Void,
    QMetaType::Void, QMetaType::QString, QMetaType::Bool,   27,   28,
    QMetaType::Void, QMetaType::QByteArray, QMetaType::Bool,   27,   28,

       0        // eod
};

void Nequeo::Net::WebSocket::Client::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Client *_t = static_cast<Client *>(_o);
        Q_UNUSED(_t)
        switch (_id) {
        case 0: _t->OnConnectedHandle(); break;
        case 1: _t->OnDisonnectedHandle(); break;
        case 2: _t->OnAboutToClose(); break;
        case 3: _t->OnTextMessageReceivedHandle((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 4: _t->OnBinaryMessageReceivedHandle((*reinterpret_cast< QByteArray(*)>(_a[1]))); break;
        case 5: _t->OnSslErrorsHandle((*reinterpret_cast< const QList<QSslError>(*)>(_a[1]))); break;
        case 6: _t->OnSocketErrorHandle((*reinterpret_cast< QAbstractSocket::SocketError(*)>(_a[1]))); break;
        case 7: _t->OnPongHandle((*reinterpret_cast< quint64(*)>(_a[1])),(*reinterpret_cast< QByteArray(*)>(_a[2]))); break;
        case 8: _t->OnStateChangedHandle((*reinterpret_cast< QAbstractSocket::SocketState(*)>(_a[1]))); break;
        case 9: _t->OnProxyAuthenticationRequiredHandle((*reinterpret_cast< const QNetworkProxy(*)>(_a[1])),(*reinterpret_cast< QAuthenticator*(*)>(_a[2]))); break;
        case 10: _t->OnReadChannelFinishedHandle(); break;
        case 11: _t->OnTextFrameReceivedHandle((*reinterpret_cast< QString(*)>(_a[1])),(*reinterpret_cast< bool(*)>(_a[2]))); break;
        case 12: _t->OnBinaryFrameReceivedHandle((*reinterpret_cast< QByteArray(*)>(_a[1])),(*reinterpret_cast< bool(*)>(_a[2]))); break;
        default: ;
        }
    }
}

const QMetaObject Nequeo::Net::WebSocket::Client::staticMetaObject = {
    { &QObject::staticMetaObject, qt_meta_stringdata_Nequeo__Net__WebSocket__Client.data,
      qt_meta_data_Nequeo__Net__WebSocket__Client,  qt_static_metacall, Q_NULLPTR, Q_NULLPTR}
};


const QMetaObject *Nequeo::Net::WebSocket::Client::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->dynamicMetaObject() : &staticMetaObject;
}

void *Nequeo::Net::WebSocket::Client::qt_metacast(const char *_clname)
{
    if (!_clname) return Q_NULLPTR;
    if (!strcmp(_clname, qt_meta_stringdata_Nequeo__Net__WebSocket__Client.stringdata0))
        return static_cast<void*>(const_cast< Client*>(this));
    return QObject::qt_metacast(_clname);
}

int Nequeo::Net::WebSocket::Client::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QObject::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 13)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 13;
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        if (_id < 13)
            *reinterpret_cast<int*>(_a[0]) = -1;
        _id -= 13;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
