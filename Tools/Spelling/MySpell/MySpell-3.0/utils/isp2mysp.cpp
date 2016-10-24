// This is a hack by Stefan Ekman <stekman@sedata.org>
// The program is not good for anything but to help
// converting a swedish ispell aff-file to a
// swedish myspell aff-file and it does not even do that
// very well. Please check the file produced. I only made this
// as I had to have a spellchecking in openoffice really fast.
// Please feel free to improve the code, but don't ask for
// improvements!
// This code is free to do anything you like with! I just needed it
// for one conversion, nothing else...

#include <iostream>
#include <fstream>
#include <string>
#include <algorithm>
#include <cstring>

char Tolower(char tkn)
{
	switch(tkn)
	{
		case 'Ä': return 'ä';
		case 'Ö': return 'ö';
		case 'Å': return 'å';
		case 'É': return 'é';
		default: return tolower(tkn);
	}
}

int main()
{
	string head;
	string tail;
	string temp;
	ifstream in("svenska.aff");
	ofstream out("sv_SE.aff");
	out << "SET ISO8859-1\nTRY aerndtislogmkpbhfjuväcöåyqxzvw\n\n";

	in >> temp;
	while(in.peek()!=EOF)
	{
		while(temp!="flag"&&in.peek()!=EOF)
		{
			in >> temp;
		}
		if(in.peek()!=EOF)
		{
			in >> temp;
			char rule=temp[1];
			char concat=temp[0]=='*'?'Y':'N';
			int nbr=0;
			tail="";
			in >> temp;
			while(temp!="flag"&&in.peek()!=EOF)
			{
				if(temp[0]!='#')
				{
					string regexp=temp;
					in >> temp;
					while(temp!=">")
					{
						regexp=regexp+temp;
						in >> temp;
					}
					transform(regexp.begin(), regexp.end(), regexp.begin(), Tolower);
					in >> temp;
					string minus;
					string plus;
					if(temp[0]=='-')
					{
						int pos=temp.find(",");
						minus=temp.substr(1,pos-1);
						plus=temp.substr(pos+1);
						if(plus=="-") plus="0";
					}
					else
					{
						minus="0";
						if(temp!="")
							plus=temp;
						else
							plus="0";
					}
					transform(plus.begin(), plus.end(), plus.begin(), Tolower);
					transform(minus.begin(),minus.end(),minus.begin(),Tolower);
					tail=tail+"SFX  "+rule+"  "+minus+"  "+plus+"  "+regexp+"\n";
					nbr++;
				}
				else
					getline(in,temp);
 				in >> temp;
			}
			out << "SFX  "<<rule<<"  "<<concat<<"  "<<nbr<<"\n";
			out << tail << endl;
		}
	}
}

