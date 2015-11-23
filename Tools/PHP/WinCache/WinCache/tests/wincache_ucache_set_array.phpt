--TEST--
Wincache - Testing wincache_ucache_* functions with arrays of values
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

echo("setting array\n");
/*
 * NOTE: setting ucache entries via an array, if the value is not defined, then
 * the index is not added to the cache.  So, 'yellow' below won't be added to
 * the WinCache ucache.
 */
$bar = array('green' => 5, 'Blue' => '6', 'yellow', 'cyan' => 'eight');

var_dump(wincache_ucache_add($bar, NULL));
var_dump(wincache_ucache_add(array('green' => 13, 'black' => 14)));
var_dump(wincache_ucache_get(array('green', 'Blue', 'cyan')));
var_dump(wincache_ucache_get('yellow')); /* should be not found! */
var_dump(wincache_ucache_set(array('green' => 9, 'Blue' => '10', 'yellow', 'cyan' => 'twelve')));
var_dump(wincache_ucache_get('green'));
var_dump(wincache_ucache_exists('green'));
var_dump(wincache_ucache_info(false, 'green'));
var_dump(wincache_ucache_delete(array('green', 'Blue', 'yellow', 'cyan')));

echo("clearing ucache\n");
var_dump(wincache_ucache_clear());

echo("setting \$foo\n");
$foo = array('green' => 5, 'Blue' => '6', 'yellow', 'cyan' => 'eight');
$foo_get = array('green', 'Blue', 'cyan');

var_dump(wincache_ucache_add($foo, NULL));
var_dump(wincache_ucache_add($foo, NULL));
var_dump(wincache_ucache_get($foo_get));
var_dump(wincache_ucache_set(array('green' => 9, 'pink' => 10, 'chartruce' => '11', 'brown', 'magenta' => 'thirteen')));
var_dump(wincache_ucache_get($foo));
var_dump(wincache_ucache_exists('pink'));
var_dump(wincache_ucache_info(false, 'pink'));
var_dump(wincache_ucache_delete($foo_get));

echo("Done!");

?>
--EXPECTF--
clearing ucache
bool(true)
setting array
array(0) {
}
array(1) {
  ["green"]=>
  int(-1)
}
array(3) {
  ["green"]=>
  int(5)
  ["Blue"]=>
  string(1) "6"
  ["cyan"]=>
  string(5) "eight"
}
bool(false)
array(0) {
}
int(9)
bool(true)
array(6) {
  ["total_cache_uptime"]=>
  int(0)
  ["is_local_cache"]=>
  bool(false)
  ["total_item_count"]=>
  int(5)
  ["total_hit_count"]=>
  int(4)
  ["total_miss_count"]=>
  int(1)
  ["ucache_entries"]=>
  array(1) {
    [1]=>
    array(6) {
      ["key_name"]=>
      string(5) "green"
      ["value_type"]=>
      string(4) "long"
      ["value_size"]=>
      int(16)
      ["ttl_seconds"]=>
      int(0)
      ["age_seconds"]=>
      int(0)
      ["hitcount"]=>
      int(1)
    }
  }
}
array(3) {
  [0]=>
  string(5) "green"
  [1]=>
  string(4) "Blue"
  [2]=>
  string(4) "cyan"
}
clearing ucache
bool(true)
setting $foo
array(0) {
}
array(4) {
  ["green"]=>
  int(-1)
  ["Blue"]=>
  int(-1)
  [0]=>
  int(-1)
  ["cyan"]=>
  int(-1)
}
array(3) {
  ["green"]=>
  int(5)
  ["Blue"]=>
  string(1) "6"
  ["cyan"]=>
  string(5) "eight"
}
array(0) {
}
array(0) {
}
bool(true)
array(6) {
  ["total_cache_uptime"]=>
  int(0)
  ["is_local_cache"]=>
  bool(false)
  ["total_item_count"]=>
  int(7)
  ["total_hit_count"]=>
  int(7)
  ["total_miss_count"]=>
  int(5)
  ["ucache_entries"]=>
  array(1) {
    [1]=>
    array(6) {
      ["key_name"]=>
      string(4) "pink"
      ["value_type"]=>
      string(4) "long"
      ["value_size"]=>
      int(16)
      ["ttl_seconds"]=>
      int(0)
      ["age_seconds"]=>
      int(0)
      ["hitcount"]=>
      int(0)
    }
  }
}
array(3) {
  [0]=>
  string(5) "green"
  [1]=>
  string(4) "Blue"
  [2]=>
  string(4) "cyan"
}
Done!