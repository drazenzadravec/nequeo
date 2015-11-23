--TEST--
Testing ini directive include_path
--SKIPIF--
<?php include('skipif.inc'); ?>
--INI--
wincache.enablecli=1
wincache.fcenabled=1
wincache.ucenabled=1
--FILE--
<?php
ini_set("include_path", ".;" . __DIR__ . "/foo");
include 'wincache_005_1.php';
ini_set("include_path", ".;" . __DIR__ . "/bar");
include 'wincache_005_2.php';
?>
==DONE==
--EXPECTF--
foo subdir
bar subdir
==DONE==


