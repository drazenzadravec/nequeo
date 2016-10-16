#pragma once

#ifndef SERY_STREAM_STD_HH_
#define SERY_STREAM_STD_HH_

///////////////////////////////////////////////////////
// This file is about standard classes serialization //
///////////////////////////////////////////////////////

#include <string>
#include <vector>
#include <list>
#include <map>
#include <set>

namespace Sery
{

// std::string
/**
 *  @brief  This overload of operator<< will serialize a std::string.
 *  @param  stream  The stream to serialize @a str to.
 *  @param  str     The std::string to serialize.
 *  @return *this.
 */
Stream&    operator<<(Stream& stream, const std::string& str);

/**
 *  @brief  This overload of operator>> will deserialize a std::string.
 *  @param      stream  The stream to deserialize @a str from.
 *  @param[out] str     The std::string to set.
 *  @return *this.
 */
Stream&    operator>>(Stream& stream, std::string& str);

// std::vector
/**
 *  @brief  This overload of operator<< will serialize a std::vector.
 *  @param  stream  The stream to serialize @a vec to.
 *  @param  vec     The std::vector to serialize.
 *  @return *this.
 */
template <class T>
Stream&    operator<<(Stream& stream, const std::vector<T>& vec)
{
  stream << static_cast<uint32>(vec.size());
  for (auto&& element : vec)
    stream << element;
  return (stream);
}
/**
 *  @brief  This overload of operator>> will deserialize a std::vector.
 *  @param      stream  The stream to deserialize @a vec from.
 *  @param[out] vec     The std::vector to set.
 *  @return *this.
 */
template <class T>
Stream&           operator>>(Stream& stream, std::vector<T>& vec)
{
  uint32          size = 0;
  std::vector<T>  tmpVec;

  stream >> size;
  tmpVec.reserve(size);
  while (size--)
  {
    T t;
    stream >> t;
    tmpVec.push_back(t);
  }
  vec = std::move(tmpVec);
  return (stream);
}

// std::list
/**
 *  @brief  This overload of operator<< will serialize a std::list.
 *  @param  stream  The stream to serialize @a list to.
 *  @param  list    The std::list to serialize.
 *  @return *this.
 */
template <class T>
Stream&    operator<<(Stream& stream, const std::list<T>& list)
{
  stream << static_cast<uint32>(list.size());
  for (auto&& element : list)
    stream << element;
  return (stream);
}
/**
 *  @brief  This overload of operator>> will deserialize a std::list.
 *  @param      stream  The stream to deserialize @a list from.
 *  @param[out] list    The std::list to set.
 *  @return *this.
 */
template <class T>
Stream&          operator>>(Stream& stream, std::list<T>& list)
{
  uint32        size = 0;
  std::list<T>  tmpList;

  stream >> size;
  while (size--)
  {
    T t;
    stream >> t;
    tmpList.push_back(t);
  }
  list = std::move(tmpList);
  return (stream);
}

// std::pair
/**
 *  @brief  This overload of operator<< will serialize a std::pair.
 *  @param  stream  The stream to serialize @a pair to.
 *  @param  pair    The std::pair to serialize.
 *  @return *this.
 */
template <class T, class U>
Stream&    operator<<(Stream& stream, const std::pair<T, U>& pair)
{
  stream << pair.first;
  stream << pair.second;
  return (stream);
}
/**
 *  @brief  This overload of operator>> will deserialize a std::pair.
 *  @param      stream  The stream to deserialize @a pair from.
 *  @param[out] pair    The std::pair to set.
 *  @return *this.
 */
template <class T, class U>
Stream&    operator>>(Stream& stream, std::pair<T, U>& pair)
{
  stream >> pair.first;
  stream >> pair.second;
  return (stream);
}

// std::map
/**
 *  @brief  This overload of operator<< will serialize a std::map.
 *  @param  stream  The stream to serialize @a map to.
 *  @param  map     The std::map to serialize.
 *  @return *this.
 */
template <class T, class U>
Stream&    operator<<(Stream& stream, const std::map<T, U>& map)
{
  stream << static_cast<uint32>(map.size());
  for (auto&& oPair : map)
    stream << oPair;
  return (stream);
}
/**
 *  @brief  This overload of operator>> will deserialize a std::map.
 *  @param      stream  The stream to deserialize @a map from.
 *  @param[out] map     The std::map to set.
 *  @return *this.
 */
template <class T, class U>
Stream&            operator>>(Stream& stream, std::map<T, U>& map)
{
  uint32          size = 0;
  std::map<T, U>  tmpMap;

  stream >> size;
  while (size--)
  {
    std::pair<T, U> pair;
    stream >> pair;
    tmpMap.insert(pair);
  }
  map = std::move(tmpMap);
  return (stream);
}

// std::set
/**
 *  @brief  This overload of operator<< will serialize a std::set.
 *  @param  stream  The stream to serialize @a set to.
 *  @param  set     The std::set to serialize.
 *  @return *this.
 */
template <class T>
Stream&    operator<<(Stream& stream, const std::set<T>& set)
{
  stream << static_cast<uint32>(set.size());
  for (auto&& element : set)
    stream << element;
  return (stream);
}
/**
 *  @brief  This overload of operator>> will deserialize a std::set.
 *  @param      stream  The stream to deserialize @a set from.
 *  @param[out] set     The std::set to set.
 *  @return *this.
 */
template <class T>
Stream&          operator>>(Stream& stream, std::set<T>& set)
{
  uint32        size = 0;
  std::set<T>   tmpSet;

  stream >> size;
  while (size--)
  {
    T t;
    stream >> t;
    tmpSet.insert(t);
  }
  set = std::move(tmpSet);
  return (stream);
}

} // namespace Sery

#endif // SERY_STREAM_STD_HH_
