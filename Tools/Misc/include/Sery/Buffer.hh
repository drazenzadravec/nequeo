#pragma once

#ifndef SERY_BUFFER_HH_
#define SERY_BUFFER_HH_

#include <vector>
#include <Sery/IBuffer.hh>

namespace Sery
{

/**
 * @brief Sery's default implementation of @ref IBuffer
 *        Represents a buffer using a std::vector<char>
 *        to store its data.
 */
class	Buffer final : public IBuffer
{
public:
  /**
   *  @brief  Default constructor
   */
  Buffer();

  /**
   *  @brief  Buffer constructor. Will initialize the internal
   *          content to that pointed by @a buffer.
   *  @param  buffer  Pointer to the byte array to store.
   *  @param  size    The length of @a buffer.
   */
  Buffer(const char* buffer, std::size_t size);

  /**
   *  @brief  Destructor.
   */
  ~Buffer();

public:
  /**
  * @brief  This function appends a raw block of memory to the internal
  *         std::vector<char>.
  * @param  buffer  Pointer to the block to append
  * @param  size    Size of the block to append
  */
  virtual void              writeRaw(const char* buffer, std::size_t size) final;

  /**
  *  @brief  This function reads a raw block of memory from the
  *          beginning of the Buffer. It effectively removes the
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
  virtual void              readRaw(char* buffer, std::size_t size) final;

  /**
  *  @brief   This function will return the current size of the Buffer.
  *  @return  The size of the internal buffer.
  */
  virtual std::size_t       size() const final;

  /**
  *  @brief  This function will return a pointer to a byte array
  *          being the buffer internally hold.
  *  @return A pointer to the internal byte array.
  *
  *  @warning  The pointer returned is read only.
  */
  virtual const char*       data() const final;

  /**
  *  @brief  This function completely empties the internal buffer.
  *          Making Buffer::size() return 0.
  */
  virtual void              clear() final;

  /**
  *  @brief  Replaces the existing content with given one.
  *  @param  buffer  A pointer to the byte array to use.
  *  @param  size    The size of @a buffer.
  */
  virtual void              setContent(const char* buffer, std::size_t size) final;

  /**
  *  @brief   This function will return a string made to represent
  *           the internal data of the buffer.
  *  @param   width The number of bytes written before inserting a
  *                 line break.
  *  @return  A string representing the data stored in the buffer.
  */
  virtual const std::string         debug(uint8 width) const final;

private:
  std::vector<char>         _buffer;    /**< Internal buffer */
};

} // namespace Sery

#endif // SERY_BUFFER_HH_