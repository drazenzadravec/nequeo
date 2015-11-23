--TEST--
Wincache - Testing wincache_ucache_* functions with IS_OBJECT data
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

/* Setting a random function should fail */

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

/* Setting a user-defined class should work */

class Connection
{
    private $dsn, $username, $password;

    public function __construct($dsn, $username, $password)
    {
        $this->dsn = $dsn;
        $this->username = $username;
        $this->password = $password;
    }

    public function __sleep()
    {
        return array('dsn', 'username', 'password');
    }

    public function __wakeup()
    {
        echo 'waking up: ' . $this . "\n";
    }

    public function __toString()
    {
        return $this->dsn . ':' . $this->username . ':*****';
    }
}

$bar2 = new Connection('MyDsn', 'MyUsername', 'MyPassword');

try
{
    var_dump(wincache_ucache_add('foo2', $bar2));
}
catch (Exception $e)
{
    var_dump($e->getMessage());
}

$bar3 = wincache_ucache_get('foo2');
echo "Fetched: {$bar3}\n";
var_dump($bar3);


echo("Done!");

?>
--EXPECTF--
clearing ucache
bool(true)
string(41) "Serialization of 'Closure' is not allowed"
bool(true)
waking up: MyDsn:MyUsername:*****
Fetched: MyDsn:MyUsername:*****
object(Connection)#4 (3) {
  ["dsn":"Connection":private]=>
  string(5) "MyDsn"
  ["username":"Connection":private]=>
  string(10) "MyUsername"
  ["password":"Connection":private]=>
  string(10) "MyPassword"
}
Done!
