--TEST--
Testing return statement from an included file
--SKIPIF--
<?php include('skipif.inc'); ?>
--INI--
wincache.enablecli=1
wincache.fcenabled=1
wincache.ucenabled=1
--FILE--
<?php
$foo = require_once 'wincache_004_require.php';
echo("$foo\r\n"); // prints 'Wincache'
?>
==DONE==
--EXPECTF--
Wincache
==DONE==

