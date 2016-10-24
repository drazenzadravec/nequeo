#!/usr/bin/perl

use locale;

open(AFF,"<./myaff");
while(<AFF>)
{
    $_ =~ s/-/0/;
    print;
}
close (AFF);








