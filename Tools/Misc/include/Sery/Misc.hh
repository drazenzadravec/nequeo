#pragma once

#ifndef SERY_MISC_HH_
#define SERY_MISC_HH_

#include <cstdint>
#include <type_traits>

namespace Sery
{

/**
 *  @brief  Shortcut for using std::enable_if.
 */
template< bool B, class T = void >
using enable_if_t = typename ::std::enable_if<B, T>::type;

/**
 *  @brief  Shortcut for using std::result_of.
 */
template< class T >
using result_of_t = typename std::result_of<T>::type;

/**
 *  @brief  Enum used to represent little of big endian.
 */
enum Endian
{
  LittleEndian,
  BigEndian
};

typedef std::int8_t   int8;   /**< Shortcut */
typedef std::int16_t  int16;  /**< Shortcut */
typedef std::int32_t  int32;  /**< Shortcut */
typedef std::int64_t  int64;  /**< Shortcut */

typedef std::uint8_t  uint8;  /**< Shortcut */
typedef std::uint16_t uint16; /**< Shortcut */
typedef std::uint32_t uint32; /**< Shortcut */
typedef std::uint64_t uint64; /**< Shortcut */


#ifndef _DOXYGEN

namespace       detail
{

  inline Endian getSoftwareEndian()
  {
    int16       witness = 0x5501;
    int8        test = *((int8*)&witness);
    return (test == 1 ? Endian::LittleEndian : Endian::BigEndian);
  }

} // namespace detail

#endif // _DOXYGEN

} // namespace Sery

#endif // SERY_MISC_HH_