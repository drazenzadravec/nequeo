#pragma once

#ifndef SERY_STREAM_HH_
#define SERY_STREAM_HH_

#include <Sery/Misc.hh>

namespace Sery
{

class IBuffer;

/**
 *  @brief  Class used to manipulate @ref IBuffer easier.
 */
class Stream final
{
private:
  // Removing copy and move functions
  Stream(const Stream&) = delete;
  Stream(Stream&&)      = delete;
  Stream& operator=(const Stream&) = delete;
  Stream& operator=(Stream&&)      = delete;

private:
  static Endian   globalEndian;   /**< Used as a default value for Stream::_localEndian */

public:
  /**
   *  @brief  Updates the global endian.
   *  @param  endian  The new endian value.
   */
  static void     setGlobalEndian(Endian endian);

  /**
   *  @brief  Gets the global endian.
   *  @return The global endian.
   */
  static Endian   getGlobalEndian();

public:
  /**
   *  @brief  Endian constructor.
   *          Allows to manipulate the given @a buffer using a specific endian
   *          (different from the global one)
   *  @param  buffer      The IBuffer to use.
   *  @param  localEndian The endian to use.
   */
  Stream(IBuffer& buffer, Endian localEndian);

  /**
   *  @brief  Basic constructor.
   *          Allows to manipulate the given @a buffer.
   *          Sets Stream::globalEndian as the localEndian.
   *  @param  buffer      The IBuffer to use.
   */
  Stream(IBuffer& buffer);

  /**
   *  @brief  Destructor.
   */
  ~Stream();

public:
  /**
   *  @brief  Proxy to call IBuffer::writeRaw.
   *  @param  buffer  The buffer to append.
   *  @param  size    The size of @a buffer.
   *
   *  @sa IBuffer::writeRaw
   */
  Stream& writeRaw(const char* buffer, uint32 size);

  /**
   *  @brief  Proxy to call IBuffer::readRaw.
   *  @param[out]  buffer  The output pointer to write the bytes.
   *                       It should be at least @a size long.
   *  @param       size    The size of @a buffer.
   *
   *  @warning  The warnings of @ref IBuffer::readRaw are still relevant.
   *
   *  @sa IBuffer::readRaw
   */
  Stream& readRaw(char* buffer, uint32 size);

public:
  /**
   *  @brief  Returns the endian used by the current Stream.
   *  @return The local endian.
   */
  Endian  getLocalEndian() const;

private:
  IBuffer&  _buffer;        /**< The buffer bound to this instance. */
  Endian    _localEndian;   /**< The endian used by this Stream. */
};


/**
 *  @brief  This overload of operator<< will serialize a boolean.
 *  @param  stream  The stream to serialize @a value to.
 *  @param  value   The boolean to serialize.
 *  @return *this.
 */
Stream& operator<<(Stream& stream, const bool value);

/**
 *  @brief  This overload of operator>> will deserialize a boolean.
 *  @param      stream  The stream to deserialize @a value from.
 *  @param[out] value   The reference to set.
 *  @return *this.
 */
Stream& operator>>(Stream& stream, bool& value);


/**
 *  @brief  This overload of operator<< will serialize a C-style string.
 *  @param  stream  The stream to serialize @a str to.
 *  @param  str     The string to serialize.
 *  @return *this.
 */
Stream& operator<<(Stream& stream, const char* str);

/**
 *  @brief  This overload of operator>> will deserialize a C-style string.
 *          The memory will be allocated by the function.
 *  @param      stream  The stream to deserialize @a str from.
 *  @param[out] str     The pointer to store the string.
 *  @return *this.
 */
Stream& operator>>(Stream& stream, char*& str);


/**
 *  @brief  This overload of operator<< will serialize an arithmetic type.
 *  @param  stream  The stream to serialize @a t to.
 *  @param  t       The arithmetic type to serialize.
 *  @return *this.
 *
 *  @warning  Thanks to SFINAE, this function won't be available for
 *            non-arithmetics types.
 *
 */
template <class T, enable_if_t<std::is_arithmetic<T>::value>* = nullptr>
Stream& operator<<(Stream& stream, T t);

/**
 *  @brief  This overload of operator>> will deserialize an arithmetic type.
 *  @param      stream  The stream to deserialize @a t from.
 *  @param[out] t       The object to set .
 *  @return *this.
 *
 *  @warning  Thanks to SFINAE, this function won't be available for
 *            non-arithmetics types.
 */
template <class T, enable_if_t<std::is_arithmetic<T>::value>* = nullptr>
Stream& operator>>(Stream& stream, T& t);

} // namespace Sery

#include "detail/Stream_STD.hh"

#endif // SERY_STREAM_HH_
