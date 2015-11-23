--TEST--
Wincache - Testing Global Variables
--SKIPIF--
<?php include('skipif.inc'); ?>
--INI--
wincache.enablecli=1
wincache.fcenabled=1
wincache.ucenabled=1
--FILE--
<?php
$variable = 2.0;
function testGlobal()
{
    global $variable;
    var_dump($variable);
}
testGlobal();
$variable += 1;
testGlobal();
$variable = "Changing to string.";
testGlobal();
?>
==DONE==
--EXPECTF--
float(2)
float(3)
string(19) "Changing to string."
==DONE==
