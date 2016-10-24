#/usr/local/bin/perl

#
# LEGAL PART
# ----------
#
# Written by François Désarménien <francois@fdesar.net>
#
# This file is PUBLIC DOMAIN and is provided "as is" without 
# any guaranty of any kind. It sole purpose is to be eventually
# usefull to someone.
#
# END OF LEGAL PART



#
# Well, beside those legal notes, it is a very basic script : it
# worked for me for compiling french (francaisGUTenberg, in fact)
# ispell affix dictionnary to myspell format. It maybe missing
# cases happenning in other foreign ispell affix files. Specially,
# it does NOT follow any strict ispell affix file syntax (in other
# words, it is not an ispell affix syntax parser, it's just a QAD
# hack)
#

use strict;
my($afxtype);
my($afxmix);
my(@flgdefs);
my($flgname);

# Constants : should be changed for languages other than french
# Check the ispell documentation for building the try list

my $SET = 'ISO8859-1';
my $TRY = 'aeioàéìòsinrtlcdugmphbyfvkw';

BEGIN {
    open(FIC, "<$ARGV[0]") or die "Cannot open '$ARGV[0]' for reading: $!";
}

END {
    close(FIC);
}

sub PrintDefs {
        @flgdefs
    or  return;

    print $afxtype, ' ', $flgname, ' ', $afxmix, ' ', scalar(@flgdefs), "\n";
    for my $def (@flgdefs) {
        #print $afxtype, ' ', $flgname, '   ', $def->{remove}, "\t",
        #      $def->{replace}, "\t", $def->{match}, "\n";
        printf "%-3.3s %-1.1s   %-10.10s %-10.10s %s\n",
               $afxtype, $flgname,  $def->{remove}, $def->{replace}, $def->{match};
    }
    print "\n";
    @flgdefs = ();
}

#
# Oooops : specific to ISO8859-1. Watch your step !
#
sub LowerCase {
    my($string) = lc(shift);

    $string =~ tr/À-ÖØ-Ý/à-öø-ý/;

    $string
}

sub ParseDef {
    my($orig) = shift;
    my($line) = $orig;
    my($match, $replace, $remove);

        $line =~ s/^([^>]+)//
    or  die "Error on matching 'match' for definition '$orig'";

    $match = LowerCase($1);
    $match =~ s/\s+//g;

    $line =~ s/^>\s+-?//;

        $line =~ s/^([^,]+),\s*//
    and $remove = LowerCase($1);

        $line =~ /^([^\s]+)(?:\s*|$)/
    or  die "Error on matching 'replace' for definition '$orig'";

    $replace = LowerCase($1);

    push(@flgdefs, { match => $match, remove => ($remove ? $remove : '0'), replace => $replace });
}

print "SET $SET\nTRY $TRY\n\n";

while(1) {
    my $line = <FIC>;

    chomp($line);

        defined($line)
    or  do {
        PrintDefs();
        last
    };

    # remove comments
    $line =~ s/\#.*$//;

    for ($line) {
        /^\s*$/ and do {
                last
            };
        /^(?:allaffixes|defstringtype|boundarychars|wordchars|stringchar|altstringtype|altstringchar)/ and do {
                last
            };
        /^prefixes\s*$/ and do {
                PrintDefs();
                $afxtype = 'PFX';
                last
            };
        /^suffixes\s*$/ and do {
                PrintDefs();
                $afxtype = 'SFX';
                last
            };

        /^flag\s+(\*)?([A-Za-z]):/ and do {
                PrintDefs();
                $afxmix = $1 eq '*' ? 'Y' : 'N';
                $flgname = $2;
                last
            };
        #
        # Maybe missing cases : add them here !!!
        #
        ParseDef($line);
    }
}
