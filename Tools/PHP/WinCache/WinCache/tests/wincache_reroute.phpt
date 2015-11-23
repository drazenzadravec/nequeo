--TEST--
Wincache - Testing Reroute
--SKIPIF--
<?php include('skipif.inc'); ?>
--INI--
wincache.enablecli=1
wincache.fcenabled=1
wincache.ucenabled=1
wincache.reroute_enabled=1
--FILE--
<?php
$directory = getcwd();
$filename = $directory."/test/ok.txt";

var_dump(mkdir($directory."/test"));
var_dump(file_exists($directory));
var_dump(is_dir($directory));

file_put_contents($filename, "Hello World!");

var_dump(file_exists($filename));
var_dump(file_get_contents($filename));
var_dump(filesize($filename));
var_dump(is_dir($filename));
var_dump(is_file($filename));
var_dump(is_readable($filename));
var_dump(is_writeable($filename));
var_dump(is_writable($filename));
var_dump(readfile($filename));
var_dump(realpath($filename));

unlink($filename);

var_dump(file_exists($filename));

rmdir($directory."/test");

var_dump(file_exists($directory."/test"));
var_dump(is_dir($directory."/test"));

?>
==DONE==
--EXPECTF--
bool(true)
bool(true)
bool(true)
bool(true)
string(%d) "Hello World!"
int(%d)
bool(false)
bool(true)
bool(true)
bool(true)
bool(true)
Hello World!int(%d)
string(%d) "%sok.txt"
bool(false)
bool(false)
bool(false)
==DONE==
