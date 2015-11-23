--TEST--
Wincache - Testing wincache_ucache_* functions with simple strings
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
$bar = "BAR";

var_dump(wincache_ucache_add('foo', $bar));
var_dump(wincache_ucache_add('foo', $bar));
var_dump(wincache_ucache_get('foo'));
var_dump(wincache_ucache_set('foo', 'BAR2'));
var_dump(wincache_ucache_get('foo'));
var_dump(wincache_ucache_exists('foo'));
var_dump(wincache_ucache_info(false, 'foo'));
var_dump(wincache_ucache_delete('foo'));

echo("setting \$foo\n");
$foo = "FOO";

var_dump(wincache_ucache_add($foo, $bar));
var_dump(wincache_ucache_add($foo, $bar));
var_dump(wincache_ucache_get($foo));
var_dump(wincache_ucache_set($foo, 'BAR2'));
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

Warning: wincache_ucache_add(): function called with a key which already exists in %swincache_ucache_string.php on line %d
bool(false)
string(3) "BAR"
bool(true)
string(4) "BAR2"
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
      string(6) "string"
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

Warning: wincache_ucache_add(): function called with a key which already exists in %swincache_ucache_string.php on line %d
bool(false)
string(3) "BAR"
bool(true)
string(4) "BAR2"
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
      string(6) "string"
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