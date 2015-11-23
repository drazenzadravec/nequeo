--TEST--

--SKIPIF--
<?php include('skipif.inc'); ?>
--INI--
wincache.enablecli=1
wincache.fcenabled=1
wincache.ucenabled=1
--FILE--
<?php
class c {
  public $p ;
  public function __get($name) { return "__get of $name" ; }
}

$c = new c ;
echo($c->p . "\r\n");    // declared public member value is empty
$c->p = 5 ;
echo($c->p . "\r\n");     // declared public member value is 5
unset($c->p) ;
echo($c->p . "\r\n");     // after unset, value is "__get of p"
?>
==DONE==
--EXPECTF--

5
__get of p
==DONE==

