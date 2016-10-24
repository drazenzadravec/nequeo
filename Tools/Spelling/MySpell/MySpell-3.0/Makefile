
CXX=g++

CXXFLAGS= -O2 -Wall -ansi -pedantic -I.

LDFLAGS=-L.  -lmyspell

LIBS=libmyspell.a

AR=ar rc
RANLIB=ranlib

OBJS = affentry.o affixmgr.o hashmgr.o suggestmgr.o csutil.o myspell.o dictmgr.o

all: example

libmyspell.a: $(OBJS)
	$(AR) $@ $(OBJS)
	-@ ($(RANLIB) $@ || true) >/dev/null 2>&1

example: example.o $(LIBS)
	$(CXX) $(CXXFLAGS) -o $@ example.o $(LDFLAGS)

%.o: %.cxx 
	$(CXX) $(CXXFLAGS) -c $<

clean:
	rm -f *.o *~ example libmyspell.a libmyspell.so*

distclean:	clean

depend:
	makedepend -- $(CXXFLAGS) -- *.[ch]xx

# DO NOT DELETE THIS LINE -- make depend depends on it.

affentry.o: affentry.hxx atypes.hxx
affixmgr.o: affentry.hxx atypes.hxx affixmgr.hxx
hashmgr.o: hashmgr.hxx htypes.hxx affixmgr.hxx
suggest.o: suggestmgr.hxx affixmgr.hxx
csutil.o: csutil.hxx
dictmgr.o: dictmgr.hxx
myspell.o: myspell.hxx suggestmgr.hxx hashmgr.hxx affixmgr.hxx csutil.hxx
example.o: myspell.hxx suggestmgr.hxx hashmgr.hxx affixmgr.hxx csutil.hxx

