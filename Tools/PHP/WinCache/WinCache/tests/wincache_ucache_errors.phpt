--TEST--
Wincache - Testing wincache_ucache_* known-bad conditions
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
$bar1 = wincache_ucache_get('foo');
var_dump(wincache_ucache_delete('foo'));
var_dump(wincache_ucache_exists('foo'));
var_dump($bar1);

$bar = function($a) {
    return $a * 2;
};

try
{
    var_dump(wincache_ucache_add('foo', $bar));
}
catch (Exception $e)
{
    var_dump($e->getMessage());
}

echo ("Done!");
?>
--EXPECTF--
clearing ucache
bool(true)
setting 'foo'
bool(true)
bool(true)
bool(false)
string(3) "BAR"
string(41) "Serialization of 'Closure' is not allowed"
Done!