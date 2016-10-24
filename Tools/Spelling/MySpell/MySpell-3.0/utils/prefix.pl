#!/usr/bin/perl

use locale;

open(AFF,"<./prefixes.aff");
while(<AFF>)
{
    if (!m/^#/ && m/\S/)
	{
	    if (m/^flag/)
	    {
		($flagline) = split(":");
		$last = substr($flagline, -1, 1);

		if ( $flagline =~ m/\*/)
		{
		    $yn = "Y";
		    $pref = "PFX ".$last;
		} else {
		    $yn = "N";
		    $pref = "PFX ".$last;
		}
		print $pref." ".$yn."\n";
	    }
	    else
	    {
		($affline) = split("#"); 
		($left, $right) = split />/, $affline;
		$rule = $left;
		($minus, $plus) = split /,/, $right;

		if ($minus =~ m/-/)
		{
		    $minus =~ s/-//; 
		} else {
		    $plus = $minus;
		    $minus = "0";
		} 

		$rule =~ s/[ \t]//g;
		$minus =~ s/[ \t]//g; $plus =~ s/[ \t]//g;  
		print $pref." ".lc($minus)." ".lc($plus)." ".lc($rule)."\n";
	    }
	}
}
close (AFF);

