--TEST--
Wincache - Testing wincache_ucache_* functions with IS_REFERENCE data
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

echo("setting 'foo' with \$bar\n");
$bar = "BAR";

var_dump(wincache_ucache_add('foo', $bar));

echo("setting 'foo2' with reference to \$bar\n");
$bar1 = &$bar;
var_dump(wincache_ucache_add('foo2', $bar1));
echo("\$bar = \"BAR++\"\n");
$bar = "BAR++";
echo("\$bar:\n");
var_dump($bar);
echo("\$bar1:\n");
var_dump($bar1);
echo("wincache_ucache_get('foo2'):\n");
var_dump(wincache_ucache_get('foo2'));

/* And now, with arrays... */

$arr = array('green' => 5, 'Blue' => '6', 'yellow', 'cyan' => 'eight');
$arr2 = &$arr;
echo("wincache_ucache_add('foo3', \$arr2)");
var_dump(wincache_ucache_add('foo3', $arr2));
echo("wincache_ucache_get('foo3')");
var_dump(wincache_ucache_get('foo3'));

/* update an element in the array, and push back into the user cache */
$arr3 = wincache_ucache_get('foo3');
echo("set \$arr3['green'] = 57\n");
$arr3['green'] = 57;
var_dump(wincache_ucache_set('foo3', $arr3));
var_dump(wincache_ucache_get('foo3'));

/* bizzare reference problem: reference to an array element, and does it
 * change arrays that were copied from the original array?
 */

$arr4 = array('pink' => 9, 'chartruce' => '10', 'brown', 'magenta' => 'twelve');

echo("\$my_ref = &\$arr4['chartruce']\n");
$my_ref = &$arr4['chartruce'];
echo("setting 'foo4' with \$arr4\n");
var_dump(wincache_ucache_add('foo4', $arr4));
echo("wincache_ucache_get('foo4'):\n");
var_dump(wincache_ucache_get('foo4'));

/* Arrays with references to other things */
$arr6 = array('black' => &$bar, 'black2' => &$bar1, 'gray' => &$arr, 'ref_to_arr' => &$arr2, 'midnight', 'dusk' => $arr3['Blue']);
echo("Nasty arr6:\n");
var_dump($arr6);
var_dump(wincache_ucache_set('foo6', $arr6));
echo("wincache_ucache_get('foo6'):\n");
var_dump(wincache_ucache_get('foo6'));

/* modify complex array and set back on top of the same key */
echo("\$arr6['gray']['Blue'] = 9\n");
$arr6['gray']['Blue'] = 9;
echo("Nasty arr6:\n");
var_dump($arr6);

echo("wincache_ucache_get('foo6'):\n");
var_dump(wincache_ucache_get('foo6'));

echo ("Done!");
?>
--EXPECTF--
clearing ucache
bool(true)
setting 'foo' with $bar
bool(true)
setting 'foo2' with reference to $bar
bool(true)
$bar = "BAR++"
$bar:
string(5) "BAR++"
$bar1:
string(5) "BAR++"
wincache_ucache_get('foo2'):
string(3) "BAR"
wincache_ucache_add('foo3', $arr2)bool(true)
wincache_ucache_get('foo3')array(4) {
  ["green"]=>
  int(5)
  ["Blue"]=>
  string(1) "6"
  [0]=>
  string(6) "yellow"
  ["cyan"]=>
  string(5) "eight"
}
set $arr3['green'] = 57
bool(true)
array(4) {
  ["green"]=>
  int(57)
  ["Blue"]=>
  string(1) "6"
  [0]=>
  string(6) "yellow"
  ["cyan"]=>
  string(5) "eight"
}
$my_ref = &$arr4['chartruce']
setting 'foo4' with $arr4
bool(true)
wincache_ucache_get('foo4'):
array(4) {
  ["pink"]=>
  int(9)
  ["chartruce"]=>
  %Sstring(2) "10"
  [0]=>
  string(5) "brown"
  ["magenta"]=>
  string(6) "twelve"
}
Nasty arr6:
array(6) {
  ["black"]=>
  &string(5) "BAR++"
  ["black2"]=>
  &string(5) "BAR++"
  ["gray"]=>
  &array(4) {
    ["green"]=>
    int(5)
    ["Blue"]=>
    string(1) "6"
    [0]=>
    string(6) "yellow"
    ["cyan"]=>
    string(5) "eight"
  }
  ["ref_to_arr"]=>
  &array(4) {
    ["green"]=>
    int(5)
    ["Blue"]=>
    string(1) "6"
    [0]=>
    string(6) "yellow"
    ["cyan"]=>
    string(5) "eight"
  }
  [0]=>
  string(8) "midnight"
  ["dusk"]=>
  string(1) "6"
}
bool(true)
wincache_ucache_get('foo6'):
array(6) {
  ["black"]=>
  &string(5) "BAR++"
  ["black2"]=>
  &string(5) "BAR++"
  ["gray"]=>
  &array(4) {
    ["green"]=>
    int(5)
    ["Blue"]=>
    string(1) "6"
    [0]=>
    string(6) "yellow"
    ["cyan"]=>
    string(5) "eight"
  }
  ["ref_to_arr"]=>
  &array(4) {
    ["green"]=>
    int(5)
    ["Blue"]=>
    string(1) "6"
    [0]=>
    string(6) "yellow"
    ["cyan"]=>
    string(5) "eight"
  }
  [0]=>
  string(8) "midnight"
  ["dusk"]=>
  string(1) "6"
}
$arr6['gray']['Blue'] = 9
Nasty arr6:
array(6) {
  ["black"]=>
  &string(5) "BAR++"
  ["black2"]=>
  &string(5) "BAR++"
  ["gray"]=>
  &array(4) {
    ["green"]=>
    int(5)
    ["Blue"]=>
    int(9)
    [0]=>
    string(6) "yellow"
    ["cyan"]=>
    string(5) "eight"
  }
  ["ref_to_arr"]=>
  &array(4) {
    ["green"]=>
    int(5)
    ["Blue"]=>
    int(9)
    [0]=>
    string(6) "yellow"
    ["cyan"]=>
    string(5) "eight"
  }
  [0]=>
  string(8) "midnight"
  ["dusk"]=>
  string(1) "6"
}
wincache_ucache_get('foo6'):
array(6) {
  ["black"]=>
  &string(5) "BAR++"
  ["black2"]=>
  &string(5) "BAR++"
  ["gray"]=>
  &array(4) {
    ["green"]=>
    int(5)
    ["Blue"]=>
    string(1) "6"
    [0]=>
    string(6) "yellow"
    ["cyan"]=>
    string(5) "eight"
  }
  ["ref_to_arr"]=>
  &array(4) {
    ["green"]=>
    int(5)
    ["Blue"]=>
    string(1) "6"
    [0]=>
    string(6) "yellow"
    ["cyan"]=>
    string(5) "eight"
  }
  [0]=>
  string(8) "midnight"
  ["dusk"]=>
  string(1) "6"
}
Done!
