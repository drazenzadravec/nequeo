/****************************************************************************
** Meta object code from reading C++ file 'httpwindow.h'
**
** Created by: The Qt Meta Object Compiler version 67 (Qt 5.7.0)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "stdafx.h"

#include "httpwindow.h"
#include <QtCore/qbytearray.h>
#include <QtCore/qmetatype.h>
#include <QtCore/QList>
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'httpwindow.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 67
#error "This file was generated using the moc from 5.7.0. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
struct qt_meta_stringdata_ProgressDialog_t {
    QByteArrayData data[5];
    char stringdata0[58];
};
#define QT_MOC_LITERAL(idx, ofs, len) \
    Q_STATIC_BYTE_ARRAY_DATA_HEADER_INITIALIZER_WITH_OFFSET(len, \
    qptrdiff(offsetof(qt_meta_stringdata_ProgressDialog_t, stringdata0) + ofs \
        - idx * sizeof(QByteArrayData)) \
    )
static const qt_meta_stringdata_ProgressDialog_t qt_meta_stringdata_ProgressDialog = {
    {
QT_MOC_LITERAL(0, 0, 14), // "ProgressDialog"
QT_MOC_LITERAL(1, 15, 20), // "networkReplyProgress"
QT_MOC_LITERAL(2, 36, 0), // ""
QT_MOC_LITERAL(3, 37, 9), // "bytesRead"
QT_MOC_LITERAL(4, 47, 10) // "totalBytes"

    },
    "ProgressDialog\0networkReplyProgress\0"
    "\0bytesRead\0totalBytes"
};
#undef QT_MOC_LITERAL

static const uint qt_meta_data_ProgressDialog[] = {

 // content:
       7,       // revision
       0,       // classname
       0,    0, // classinfo
       1,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: name, argc, parameters, tag, flags
       1,    2,   19,    2, 0x0a /* Public */,

 // slots: parameters
    QMetaType::Void, QMetaType::LongLong, QMetaType::LongLong,    3,    4,

       0        // eod
};

void ProgressDialog::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        ProgressDialog *_t = static_cast<ProgressDialog *>(_o);
        Q_UNUSED(_t)
        switch (_id) {
        case 0: _t->networkReplyProgress((*reinterpret_cast< qint64(*)>(_a[1])),(*reinterpret_cast< qint64(*)>(_a[2]))); break;
        default: ;
        }
    }
}

const QMetaObject ProgressDialog::staticMetaObject = {
    { &QProgressDialog::staticMetaObject, qt_meta_stringdata_ProgressDialog.data,
      qt_meta_data_ProgressDialog,  qt_static_metacall, Q_NULLPTR, Q_NULLPTR}
};


const QMetaObject *ProgressDialog::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->dynamicMetaObject() : &staticMetaObject;
}

void *ProgressDialog::qt_metacast(const char *_clname)
{
    if (!_clname) return Q_NULLPTR;
    if (!strcmp(_clname, qt_meta_stringdata_ProgressDialog.stringdata0))
        return static_cast<void*>(const_cast< ProgressDialog*>(this));
    return QProgressDialog::qt_metacast(_clname);
}

int ProgressDialog::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QProgressDialog::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 1)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 1;
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        if (_id < 1)
            *reinterpret_cast<int*>(_a[0]) = -1;
        _id -= 1;
    }
    return _id;
}
struct qt_meta_stringdata_HttpWindow_t {
    QByteArrayData data[13];
    char stringdata0[180];
};
#define QT_MOC_LITERAL(idx, ofs, len) \
    Q_STATIC_BYTE_ARRAY_DATA_HEADER_INITIALIZER_WITH_OFFSET(len, \
    qptrdiff(offsetof(qt_meta_stringdata_HttpWindow_t, stringdata0) + ofs \
        - idx * sizeof(QByteArrayData)) \
    )
static const qt_meta_stringdata_HttpWindow_t qt_meta_stringdata_HttpWindow = {
    {
QT_MOC_LITERAL(0, 0, 10), // "HttpWindow"
QT_MOC_LITERAL(1, 11, 12), // "downloadFile"
QT_MOC_LITERAL(2, 24, 0), // ""
QT_MOC_LITERAL(3, 25, 14), // "cancelDownload"
QT_MOC_LITERAL(4, 40, 12), // "httpFinished"
QT_MOC_LITERAL(5, 53, 13), // "httpReadyRead"
QT_MOC_LITERAL(6, 67, 20), // "enableDownloadButton"
QT_MOC_LITERAL(7, 88, 26), // "slotAuthenticationRequired"
QT_MOC_LITERAL(8, 115, 14), // "QNetworkReply*"
QT_MOC_LITERAL(9, 130, 15), // "QAuthenticator*"
QT_MOC_LITERAL(10, 146, 9), // "sslErrors"
QT_MOC_LITERAL(11, 156, 16), // "QList<QSslError>"
QT_MOC_LITERAL(12, 173, 6) // "errors"

    },
    "HttpWindow\0downloadFile\0\0cancelDownload\0"
    "httpFinished\0httpReadyRead\0"
    "enableDownloadButton\0slotAuthenticationRequired\0"
    "QNetworkReply*\0QAuthenticator*\0sslErrors\0"
    "QList<QSslError>\0errors"
};
#undef QT_MOC_LITERAL

static const uint qt_meta_data_HttpWindow[] = {

 // content:
       7,       // revision
       0,       // classname
       0,    0, // classinfo
       7,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: name, argc, parameters, tag, flags
       1,    0,   49,    2, 0x08 /* Private */,
       3,    0,   50,    2, 0x08 /* Private */,
       4,    0,   51,    2, 0x08 /* Private */,
       5,    0,   52,    2, 0x08 /* Private */,
       6,    0,   53,    2, 0x08 /* Private */,
       7,    2,   54,    2, 0x08 /* Private */,
      10,    2,   59,    2, 0x08 /* Private */,

 // slots: parameters
    QMetaType::Void,
    QMetaType::Void,
    QMetaType::Void,
    QMetaType::Void,
    QMetaType::Void,
    QMetaType::Void, 0x80000000 | 8, 0x80000000 | 9,    2,    2,
    QMetaType::Void, 0x80000000 | 8, 0x80000000 | 11,    2,   12,

       0        // eod
};

void HttpWindow::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        HttpWindow *_t = static_cast<HttpWindow *>(_o);
        Q_UNUSED(_t)
        switch (_id) {
        case 0: _t->downloadFile(); break;
        case 1: _t->cancelDownload(); break;
        case 2: _t->httpFinished(); break;
        case 3: _t->httpReadyRead(); break;
        case 4: _t->enableDownloadButton(); break;
        case 5: _t->slotAuthenticationRequired((*reinterpret_cast< QNetworkReply*(*)>(_a[1])),(*reinterpret_cast< QAuthenticator*(*)>(_a[2]))); break;
        case 6: _t->sslErrors((*reinterpret_cast< QNetworkReply*(*)>(_a[1])),(*reinterpret_cast< const QList<QSslError>(*)>(_a[2]))); break;
        default: ;
        }
    }
}

const QMetaObject HttpWindow::staticMetaObject = {
    { &QDialog::staticMetaObject, qt_meta_stringdata_HttpWindow.data,
      qt_meta_data_HttpWindow,  qt_static_metacall, Q_NULLPTR, Q_NULLPTR}
};


const QMetaObject *HttpWindow::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->dynamicMetaObject() : &staticMetaObject;
}

void *HttpWindow::qt_metacast(const char *_clname)
{
    if (!_clname) return Q_NULLPTR;
    if (!strcmp(_clname, qt_meta_stringdata_HttpWindow.stringdata0))
        return static_cast<void*>(const_cast< HttpWindow*>(this));
    return QDialog::qt_metacast(_clname);
}

int HttpWindow::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QDialog::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 7)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 7;
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        if (_id < 7)
            *reinterpret_cast<int*>(_a[0]) = -1;
        _id -= 7;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
