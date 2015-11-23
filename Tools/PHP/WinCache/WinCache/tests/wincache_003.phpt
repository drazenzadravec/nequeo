--TEST--
Testing require directive
--SKIPIF--
<?php include('skipif.inc'); ?>
--INI--
wincache.enablecli=1
wincache.fcenabled=1
wincache.ucenabled=1
--FILE--
<?php
$fruit = '';
function foo()
{
    global $color;
    if ((require 'wincache_require.php') == 1)
        echo("A $color $fruit\r\n");
}

foo();                         // A green apple
echo("A $color $fruit\r\n");   // A green
?>
==DONE==
--EXPECTF--
A green apple
A green 
==DONE==

