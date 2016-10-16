#pragma once

#ifndef SERY_IBUFFER_HH_
#define SERY_IBUFFER_HH_

#include <Sery/Misc.hh>
#include <string>
#include <memory>
#include <functional>

namespace Sery
{

/**
 * @brief  Base class for Sery::Buffer, represents a byte array
 *         manipulation interface.
 */
class	IBuffer
{
public:
  /**
   *  @brief  Virtual destructor for inheritance.
   */
  virtual ~IBuffer()
  {
  }

public:
  /**
   * @brief  This function appends a raw block of memory to the IBuffer
   * @param  buffer  Pointer to the block to append
   * @param  size    Size of the block to append
   */
  virtual void              writeRaw(const char* buffer, std::size_t size) = 0;

  /**
   *  @brief  This function reads a raw block of memory from the
   *          beginning of the IBuffer. It effectively removes the
   *          @a size first bytes of the internal buffer.
   *
   *  @param[out]   buffer  The output pointer to write the bytes.
   *                        It should be at least @a size long.
   *  @param        size    Size of the block to read
   *
   *  @warning  This function expects @a buffer to be allocated
   *            before the call. Otherwise, it will result in
   *            undefined behavior.
   *
   *  @warning  If @a size is greater than IBuffer::size(), this
   *            is undefined behavior.
   */
  virtual void              readRaw(char* buffer, std::size_t size) = 0;

  /**
   *  @brief   This function will return the current size of the IBuffer.
   *  @return  The size of the internal buffer.
   */
  virtual std::size_t       size() const = 0;

  /**
   *  @brief  This function will return a pointer to a byte array
   *          being the buffer internally hold.
   *  @return A pointer to the internal byte array.
   *
   *  @warning  The pointer returned is read only.
   */
  virtual const char*       data() const = 0;

  /**
   *  @brief  This function completely empties the internal buffer.
   *          Making IBuffer::size() return 0.
   */
  virtual void              clear() = 0;

  /**
   *  @brief  Replaces the existing content with given one.
   *  @param  buffer  A pointer to the byte array to use.
   *  @param  size    The size of @a buffer.
   */
  virtual void              setContent(const char* buffer, std::size_t size) = 0;

  /**
  *  @brief   This function will return a string made to represent
  *           the internal data of the buffer.
  *  @param   width The number of bytes written before inserting a
  *                 line break.
  *  @return  A string representing the data stored in the buffer.
  */
  virtual const std::string debug(uint8 width) const = 0;
};

/**
 *  @brief  This function is a helper meant to be used with a read-like
 *          callable.
 *
 *  @param  callable    A callable parameter taking two parameters.
 *                      It must be able to take a char * and an IntType.
 *  @param  buffer      The @ref IBuffer to set the content of.
 *  @param  sizeToRead  The size to read from @a callable.
 *
 *  This function will allocate a raw char array and call @a callable
 *  with it. Here is an example passing a lambda :
 *
 *  @code{.cpp}
 *    Sery::Buffer    buffer;
 *    istringstream   stream("Dummy stream");
 *    Sery::readToBuffer([&stream] (char* buf, int size) {
 *                         stream.read(buf, size);
 *                       },
 *                       buffer, 5);
 *    std::cout << buffer.size() << "\n"; // Prints "5";
 *  @endcode
 *
 *  This example will call strea.read to read 5 bytes.
 *  The read bytes will directly be stored inside buffer and
 *  calling buffer.size() after the call will return 5.
 *
 *  @note The template IntType is not needed and is only used
 *        to remove a warning from MSVC.
 *
 */
template < typename Callable, typename IntType >
void  readToBuffer(Callable callable,
                   IBuffer& buffer,
                   IntType sizeToRead)
{
  std::unique_ptr<char[]> charBuffer(new char[sizeToRead]);

  callable(charBuffer.get(), sizeToRead);
  buffer.setContent(charBuffer.get(), sizeToRead);
}

} // namespace Sery

#endif // SERY_IBUFFER_HH_
