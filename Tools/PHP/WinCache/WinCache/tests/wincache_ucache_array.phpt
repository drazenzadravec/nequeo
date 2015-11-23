--TEST--
Wincache - Testing wincache_ucache_* functions with IS_ARRAY data
--SKIPIF--
<?php include('skipif.inc'); ?>
--INI--
wincache.enablecli=1
wincache.fcenabled=1
wincache.ucenabled=1
--FILE--
<?php

echo("clearing ucache\n");
var_dump(wincache_ucache_clear());

echo("setting 'foo'\n");
$bar = array('green' => 5, 'Blue' => '6', 'yellow', 'cyan' => 'eight');

var_dump(wincache_ucache_add('foo', $bar));
var_dump(wincache_ucache_add('foo', $bar));
var_dump(wincache_ucache_get('foo'));
var_dump(wincache_ucache_set('foo', array('pink' => 9, 'chartruce' => '10', 'brown', 'magenta' => 'twelve')));
var_dump(wincache_ucache_get('foo'));
var_dump(wincache_ucache_exists('foo'));
var_dump(wincache_ucache_info(false, 'foo'));
var_dump(wincache_ucache_delete('foo'));

echo("setting \$foo\n");
$foo = "FOO";

var_dump(wincache_ucache_add($foo, $bar));
var_dump(wincache_ucache_add($foo, $bar));
var_dump(wincache_ucache_get($foo));
var_dump(wincache_ucache_set($foo, array('pink' => 9, 'chartruce' => '10', 'brown', 'magenta' => 'twelve')));
var_dump(wincache_ucache_get($foo));
var_dump(wincache_ucache_exists($foo));
var_dump(wincache_ucache_info(false, $foo));
var_dump(wincache_ucache_delete($foo));

echo("Done!");

?>
--EXPECTF--
clearing ucache
bool(true)
setting 'foo'
bool(true)

Warning: wincache_ucache_add(): function called with a key which already exists in %s on line %d
bool(false)
array(4) {
  ["green"]=>
  int(5)
  ["Blue"]=>
  string(1) "6"
  [0]=>
  string(6) "yellow"
  ["cyan"]=>
  string(5) "eight"
}
bool(true)
array(4) {
  ["pink"]=>
  int(9)
  ["chartruce"]=>
  string(2) "10"
  [0]=>
  string(5) "brown"
  ["magenta"]=>
  string(6) "twelve"
}
bool(true)
array(6) {
  ["total_cache_uptime"]=>
  int(0)
  ["is_local_cache"]=>
  bool(false)
  ["total_item_count"]=>
  int(1)
  ["total_hit_count"]=>
  int(2)
  ["total_miss_count"]=>
  int(0)
  ["ucache_entries"]=>
  array(1) {
    [1]=>
    array(6) {
      ["key_name"]=>
      string(3) "foo"
      ["value_type"]=>
      string(5) "array"
      ["value_size"]=>
      int(%d)
      ["ttl_seconds"]=>
      int(0)
      ["age_seconds"]=>
      int(0)
      ["hitcount"]=>
      int(1)
    }
  }
}
bool(true)
setting $foo
bool(true)

Warning: wincache_ucache_add(): function called with a key which already exists in %s on line %d
bool(false)
array(4) {
  ["green"]=>
  int(5)
  ["Blue"]=>
  string(1) "6"
  [0]=>
  string(6) "yellow"
  ["cyan"]=>
  string(5) "eight"
}
bool(true)
array(4) {
  ["pink"]=>
  int(9)
  ["chartruce"]=>
  string(2) "10"
  [0]=>
  string(5) "brown"
  ["magenta"]=>
  string(6) "twelve"
}
bool(true)
array(6) {
  ["total_cache_uptime"]=>
  int(0)
  ["is_local_cache"]=>
  bool(false)
  ["total_item_count"]=>
  int(1)
  ["total_hit_count"]=>
  int(4)
  ["total_miss_count"]=>
  int(0)
  ["ucache_entries"]=>
  array(1) {
    [1]=>
    array(6) {
      ["key_name"]=>
      string(3) "FOO"
      ["value_type"]=>
      string(5) "array"
      ["value_size"]=>
      int(%d)
      ["ttl_seconds"]=>
      int(0)
      ["age_seconds"]=>
      int(0)
      ["hitcount"]=>
      int(1)
    }
  }
}
bool(true)
Done!
