--TEST--
Testing static variables inside class
--SKIPIF--
<?php include('skipif.inc'); ?>
--INI--
wincache.enablecli=1
wincache.fcenabled=1
wincache.ucenabled=1
--FILE--
<?php

class MyParent {

    protected static $variable;
}

class Child1 extends MyParent {

    function set() {
        self::$variable = 2;
    }
}

class Child2 extends MyParent {

    function show() {
        echo(self::$variable . "\r\n");
    }
}

$c1 = new Child1();
$c1->set();
$c2 = new Child2();
$c2->show(); // prints 2
?>
==DONE==
--EXPECTF--
2
==DONE==

