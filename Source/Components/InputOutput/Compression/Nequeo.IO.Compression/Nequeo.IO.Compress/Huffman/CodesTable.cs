/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.IO.Compression.Huffman
{
    using Map = Dictionary<bool[], byte>;

    /// <summary>
    /// Codes table.
    /// </summary>
    internal class CodesTable
    {
        // see spec 07 - > 4.1.2.  String Literal Representation
        // String literals which use Huffman encoding are encoded with the
        // Huffman codes defined in Appendix C

        #region Huffman Codes Table

        private Map _symbolBitsMap = new Map
            {
                {new[] {T,T,T,T,T,T,T,T, T,T,F,F,F}, (byte) 0},                                         //'' (0) |11111111|11000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,T,F,F,F}, (byte) 1},                    //'' (1) |11111111|11111111|1011000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,F,T,F}, (byte) 2},         //'' (2) |11111111|11111111|11111110|0010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,F,T,T}, (byte) 3},         //'' (3) |11111111|11111111|11111110|0011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,T,F,F}, (byte) 4},         //'' (4) |11111111|11111111|11111110|0100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,T,F,T}, (byte) 5},         //'' (5) |11111111|11111111|11111110|0101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,T,T,F}, (byte) 6},         //'' (6) |11111111|11111111|11111110|0110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,T,T,T}, (byte) 7},         //'' (7) |11111111|11111111|11111110|0111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,F,F,F}, (byte) 8},         //'' (8) |11111111|11111111|11111110|1000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,T,F,T,F}, (byte) 9},                  //'' (9) |11111111|11111111|11101010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,F}, (byte) 10},    //'' (10) |11111111|11111111|11111111|111100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,F,F,T}, (byte) 11},        //'' (11) |11111111|11111111|11111110|1001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,F,T,F}, (byte) 12},        //'' (12) |11111111|11111111|11111110|1010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,T}, (byte) 13},    //'' (13) |11111111|11111111|11111111|111101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,F,T,T}, (byte) 14},        //'' (14) |11111111|11111111|11111110|1011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,T,F,F}, (byte) 15},        //'' (15) |11111111|11111111|11111110|1100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,T,F,T}, (byte) 16},        //'' (16) |11111111|11111111|11111110|1101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,T,T,F}, (byte) 17},        //'' (17) |11111111|11111111|11111110|1110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,T,T,T}, (byte) 18},        //'' (18) |11111111|11111111|11111110|1111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,F,F}, (byte) 19},        //'' (19) |11111111|11111111|11111111|0000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,F,T}, (byte) 20},        //'' (20) |11111111|11111111|11111111|0001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,T,F}, (byte) 21},        //'' (21) |11111111|11111111|11111111|0010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F}, (byte) 22},    //'' (22) |11111111|11111111|11111111|111110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,T,T}, (byte) 23},        //'' (23) |11111111|11111111|11111111|0011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,F,F}, (byte) 24},        //'' (24) |11111111|11111111|11111111|0100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,F,T}, (byte) 25},        //'' (25) |11111111|11111111|11111111|0101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,T,F}, (byte) 26},        //'' (26) |11111111|11111111|11111111|0110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,T,T}, (byte) 27},        //'' (27) |11111111|11111111|11111111|0111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,F,F}, (byte) 28},        //'' (28) |11111111|11111111|11111111|1000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,F,T}, (byte) 29},        //'' (29) |11111111|11111111|11111111|1001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,F}, (byte) 30},        //'' (30) |11111111|11111111|11111111|1010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,T}, (byte) 31},        //'' (31) |11111111|11111111|11111111|1011
                {new[] {F,T,F,T,F,F}, (byte) 32},                                                       //' ' (32) |010100
                {new[] {T,T,T,T,T,T,T,F, F,F}, (byte) 33},                                              //'!' (33) |11111110|00
                {new[] {T,T,T,T,T,T,T,F, F,T}, (byte) 34},                                              //'"' (34) |11111110|01
                {new[] {T,T,T,T,T,T,T,T, T,F,T,F}, (byte) 35},                                          //'#' (35) |11111111|1010
                {new[] {T,T,T,T,T,T,T,T, T,T,F,F,T}, (byte) 36},                                        //'$' (36) |11111111|11001
                {new[] {F,T,F,T,F,T}, (byte) 37},                                                       //'%' (37) |010101
                {new[] {T,T,T,T,T,F,F,F}, (byte) 38},                                                   //'&' (38) |11111000
                {new[] {T,T,T,T,T,T,T,T, F,T,F}, (byte) 39},                                            //''' (39) |11111111|010
                {new[] {T,T,T,T,T,T,T,F, T,F}, (byte) 40},                                              //'(' (40) |11111110|10
                {new[] {T,T,T,T,T,T,T,F, T,T}, (byte) 41},                                              //')' (41) |11111110|11
                {new[] {T,T,T,T,T,F,F,T}, (byte) 42},                                                   //'*' (42) |11111001
                {new[] {T,T,T,T,T,T,T,T, F,T,T}, (byte) 43},                                            //'+' (43) |11111111|011
                {new[] {T,T,T,T,T,F,T,F}, (byte) 44},                                                   //',' (44) |11111010
                {new[] {F,T,F,T,T,F}, (byte) 45},                                                       //'-' (45) |010110
                {new[] {F,T,F,T,T,T}, (byte) 46},                                                       //'.' (46) |010111
                {new[] {F,T,T,F,F,F}, (byte) 47},                                                       //'/' (47) |011000
                {new[] {F,F,F,F,F}, (byte) 48},                                                         //'0' (48) |00000                    
                {new[] {F,F,F,F,T}, (byte) 49},                                                         //'1' (49) |00001
                {new[] {F,F,F,T,F}, (byte) 50},                                                         //'2' (50) |00010
                {new[] {F,T,T,F,F,T}, (byte) 51},                                                       //'3' (51) |011001
                {new[] {F,T,T,F,T,F}, (byte) 52},                                                       //'4' (52) |011010
                {new[] {F,T,T,F,T,T}, (byte) 53},                                                       //'5' (53) |011011
                {new[] {F,T,T,T,F,F}, (byte) 54},                                                       //'6' (54) |011100
                {new[] {F,T,T,T,F,T}, (byte) 55},                                                       //'7' (55) |011101
                {new[] {F,T,T,T,T,F}, (byte) 56},                                                       //'8' (56) |011110
                {new[] {F,T,T,T,T,T}, (byte) 57},                                                       //'9' (57) |011111
                {new[] {T,F,T,T,T,F,F}, (byte) 58},                                                     //':' (58) |1011100
                {new[] {T,T,T,T,T,F,T,T}, (byte) 59},                                                   //';' (59) |11111011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,F,F}, (byte) 60},                                    //'<' (60) |11111111|1111100
                {new[] {T,F,F,F,F,F}, (byte) 61},                                                       //'=' (61) |100000
                {new[] {T,T,T,T,T,T,T,T, T,F,T,T}, (byte) 62},                                          //'>' (62) |11111111|1011
                {new[] {T,T,T,T,T,T,T,T ,F,F}, (byte) 63},                                              //'?' (63) |11111111|00
                {new[] {T,T,T,T,T,T,T,T, T,T,F,T,F}, (byte) 64},                                        //'@' (64) |11111111|11010
                {new[] {T,F,F,F,T}, (byte) 65},                                                         //'A' (65) |100001
                {new[] {T,F,T,T,T,F,T}, (byte) 66},                                                     //'B' (66) |1011101
                {new[] {T,F,T,T,T,T,F}, (byte) 67},                                                     //'C' (67) |1011110
                {new[] {T,F,T,T,T,T,T}, (byte) 68},                                                     //'D' (68) |1011111
                {new[] {T,T,F,F,F,F,F}, (byte) 69},                                                     //'E' (69) |1100000
                {new[] {T,T,F,F,F,F,T}, (byte) 70},                                                     //'F' (70) |1100001
                {new[] {T,T,F,F,F,T,F}, (byte) 71},                                                     //'G' (71) |1100010
                {new[] {T,T,F,F,F,T,T}, (byte) 72},                                                     //'H' (72) |1100011
                {new[] {T,T,F,F,T,F,F}, (byte) 73},                                                     //'I' (73) |1100100
                {new[] {T,T,F,F,T,F,T}, (byte) 74},                                                     //'J' (74) |1100101
                {new[] {T,T,F,F,T,T,F}, (byte) 75},                                                     //'K' (75) |1100110
                {new[] {T,T,F,F,T,T,T}, (byte) 76},                                                     //'L' (76) |1100111
                {new[] {T,T,F,T,F,F,F}, (byte) 77},                                                     //'M' (77) |1101000
                {new[] {T,T,F,T,F,F,T}, (byte) 78},                                                     //'N' (78) |1101001  
                {new[] {T,T,F,T,F,T,F}, (byte) 79},                                                     //'O' (79) |1101010
                {new[] {T,T,F,T,F,T,T}, (byte) 80},                                                     //'P' (80) |1101011
                {new[] {T,T,F,T,T,F,F}, (byte) 81},                                                     //'Q' (81) |1101100
                {new[] {T,T,F,T,T,F,T}, (byte) 82},                                                     //'R' (82) |1101101
                {new[] {T,T,F,T,T,T,F}, (byte) 83},                                                     //'S' (83) |1101110
                {new[] {T,T,F,T,T,T,T}, (byte) 84},                                                     //'T' (84) |1101111
                {new[] {T,T,T,F,F,F,F}, (byte) 85},                                                     //'U' (85) |1110000
                {new[] {T,T,T,F,F,F,T}, (byte) 86},                                                     //'V' (86) |1110001
                {new[] {T,T,T,F,F,T,F}, (byte) 87},                                                     //'W' (87) |1110010
                {new[] {T,T,T,T,T,T,F,F}, (byte) 88},                                                   //'X' (88) |11111100
                {new[] {T,T,T,F,F,T,T}, (byte) 89},                                                     //'Y' (89) |1110011
                {new[] {T,T,T,T,T,T,F,T}, (byte) 90},                                                   //'Z' (90) |11111101
                {new[] {T,T,T,T,T,T,T,T, T,T,F,T,T}, (byte) 91},                                        //'[' (91) |11111111|11011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,F,F}, (byte) 92},                           //'\' (92) |11111111|11111110|000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,F,F}, (byte) 93},                                        //']' (93) |11111111|11100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,F,F}, (byte) 94},                                      //'^' (94) |11111111|111100
                {new[] {T,F,F,F,T,F}, (byte) 95},                                                       //'_' (95) |100010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,F,T}, (byte) 96},                                    //'`' (96) |11111111|1111101
                {new[] {F,F,F,T,T}, (byte) 97},                                                         //'a' (97) |00011
                {new[] {T,F,F,F,T,T}, (byte) 98},                                                       //'b' (98) |100011
                {new[] {F,F,T,F,F}, (byte) 99},                                                         //'c' (99) |00100
                {new[] {T,F,F,T,F,F}, (byte) 100},                                                      //'d' (100) |100100
                {new[] {F,F,T,F,T}, (byte) 101},                                                        //'e' (101) |00101
                {new[] {T,F,F,T,F,T}, (byte) 102},                                                      //'f' (102) |100101
                {new[] {T,F,F,T,T,F}, (byte) 103},                                                      //'g' (103) |100110
                {new[] {T,F,F,T,T,T}, (byte) 104},                                                      //'h' (104) |100111
                {new[] {F,F,T,T,F}, (byte) 105},                                                        //'i' (105) |00110
                {new[] {T,T,T,F,T,F,F}, (byte) 106},                                                    //'j' (106) |1110100
                {new[] {T,T,T,F,T,F,T}, (byte) 107},                                                    //'k' (107) |1110101
                {new[] {T,F,T,F,F,F}, (byte) 108},                                                      //'l' (108) |101000 
                {new[] {T,F,T,F,F,T}, (byte) 109},                                                      //'m' (109) |101001
                {new[] {T,F,T,F,T,F}, (byte) 110},                                                      //'n' (110) |101010
                {new[] {F,F,T,T,T}, (byte) 111},                                                        //'o' (111) |00111
                {new[] {T,F,T,F,T,T}, (byte) 112},                                                      //'p' (112) |101011
                {new[] {T,T,T,F,T,T,F}, (byte) 113},                                                    //'q' (113) |1110110
                {new[] {T,F,T,T,F,F}, (byte) 114},                                                      //'r' (114) |101100
                {new[] {F,T,F,F,F}, (byte) 115},                                                        //'s' (115) |01000
                {new[] {F,T,F,F,T}, (byte) 116},                                                        //'t' (116) |01001
                {new[] {T,F,T,T,F,T}, (byte) 117},                                                      //'u' (117) |101101
                {new[] {T,T,T,F,T,T,T}, (byte) 118},                                                    //'v' (118) |1110111
                {new[] {T,T,T,T,F,F,F}, (byte) 119},                                                    //'w' (119) |1111000
                {new[] {T,T,T,T,F,F,T}, (byte) 120},                                                    //'x' (120) |1111001
                {new[] {T,T,T,T,F,T,F}, (byte) 121},                                                    //'y' (121) |1111010
                {new[] {T,T,T,T,F,T,T}, (byte) 122},                                                    //'z' (122) |1111011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,F}, (byte) 123},                                   //'{' (123) |11111111|1111110
                {new[] {T,T,T,T,T,T,T,T, T,F,F}, (byte) 124},                                           //'|' (124) |11111111|100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,F,T}, (byte) 125},                                     //'}' (125) |11111111|111101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,F,T}, (byte) 126},                                       //'~' (126) |11111111|11101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,F}, (byte) 127},       //'' (127) |11111111|11111111|11111111|1100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,T,T,F}, (byte) 128},                        //'' (128) |11111111|11111110|0110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,F,F,T,F}, (byte) 129},                    //'' (129) |11111111|11111111|010010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,T,T,T}, (byte) 130},                        //'' (130) |11111111|11111110|0111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,F,F,F}, (byte) 131},                        //'' (131) |11111111|11111110|1000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,F,F,T,T}, (byte) 132},                    //'' (132) |11111111|11111111|010011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,F,T,F,F}, (byte) 133},                    //'' (133) |11111111|11111111|010100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,F,T,F,T}, (byte) 134},                    //'' (134) |11111111|11111111|010101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,T,F,F,T}, (byte) 135},                  //'' (135) |11111111|11111111|1011001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,F,T,T,F}, (byte) 136},                    //'' (136) |11111111|11111111|010110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,T,F,T,F}, (byte) 137},                  //'' (137) |11111111|11111111|1011010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,T,F,T,T}, (byte) 138},                  //'' (138) |11111111|11111111|1011011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,T,T,F,F}, (byte) 139},                  //'' (139) |11111111|11111111|1011100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,T,T,F,T}, (byte) 140},                  //'' (140) |11111111|11111111|1011101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,T,T,T,F}, (byte) 141},                  //'' (141) |11111111|11111111|1011110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,T,F,T,T}, (byte) 142},                //'' (142) |11111111|11111111|11101011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,T,T,T,T}, (byte) 143},                  //'' (143) |11111111|11111111|1011111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,T,T,F,F}, (byte) 144},                //'' (144) |11111111|11111111|11101100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,T,T,F,T}, (byte) 145},                //'' (145) |11111111|11111111|11101101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,F,T,T,T}, (byte) 146},                    //'' (146) |11111111|11111111|010111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,F,F,F,F}, (byte) 147},                  //'' (147) |11111111|11111111|1100000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,T,T,T,F}, (byte) 148},                //'' (148) |11111111|11111111|11101110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,F,F,F,T}, (byte) 149},                  //'' (149) |11111111|11111111|1100001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,F,F,T,F}, (byte) 150},                  //'' (150) |11111111|11111111|1100010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,F,F,T,T}, (byte) 151},                  //'' (151) |11111111|11111111|1100011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,F,T,F,F}, (byte) 152},                  //'' (152) |11111111|11111111|1100100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,T,T,F,F}, (byte) 153},                      //'' (153) |11111111|11111110|11100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,T,F,F,F}, (byte) 154},                    //'' (154) |11111111|11111111|011000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,F,T,F,T}, (byte) 155},                  //'' (155) |11111111|11111111|1100101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,T,F,F,T}, (byte) 156},                    //'' (156) |11111111|11111111|011001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,F,T,T,F}, (byte) 157},                  //'' (157) |11111111|11111111|1100110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,F,T,T,T}, (byte) 158},                  //'' (158)  |11111111|11111111|1100111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,T,T,T,T}, (byte) 159},                //'' (159)  |11111111|11111111|11101111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,T,F,T,F}, (byte) 160},                    //'' (160)  |11111111|11111111|011010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,T,T,F,T}, (byte) 161},                      //'' (161)  |11111111|11111110|11101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,F,F,T}, (byte) 162},                        //'' (162)  |11111111|11111110|1001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,T,F,T,T}, (byte) 163},                    //'' (163)  |11111111|11111111|011011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,T,T,F,F}, (byte) 164},                    //'' (164)  |11111111|11111111|011100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,T,F,F,F}, (byte) 165},                  //'' (165)  |11111111|11111111|1101000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,T,F,F,T}, (byte) 166},                  //'' (166)  |11111111|11111111|1101001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,T,T,T,F}, (byte) 167},                      //'' (167)  |11111111|11111110|11110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,T,F,T,F}, (byte) 168},                  //'' (168)  |11111111|11111111|1101010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,T,T,F,T}, (byte) 169},                    //'' (169)  |11111111|11111111|011101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,T,T,T,F}, (byte) 170},                    //'' (170)  |11111111|11111111|011110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,F,F,F}, (byte) 171},                //'' (171)  |11111111|11111111|11110000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,T,T,T,T}, (byte) 172},                      //'' (172)  |11111111|11111110|11111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,T,T,T,T}, (byte) 173},                    //'' (173)  |11111111|11111111|011111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,T,F,T,T}, (byte) 174},                  //'' (174)  |11111111|11111111|1101011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,T,T,F,F}, (byte) 175},                  //'' (175)  |11111111|11111111|1101100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,F,F,F}, (byte) 176},                      //'' (176)  |11111111|11111111|00000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,F,F,T}, (byte) 177},                      //'' (177)  |11111111|11111111|00001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,F,F,F,F}, (byte) 178},                    //'' (178)  |11111111|11111111|100000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,F,T,F}, (byte) 179},                      //'' (179)  |11111111|11111111|00010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,T,T,F,T}, (byte) 180},                  //'' (180)  |11111111|11111111|1101101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,F,F,F,T}, (byte) 181},                    //'' (181)  |11111111|11111111|100001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,T,T,T,F}, (byte) 182},                  //'' (182)  |11111111|11111111|1101110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,T,T,T,T}, (byte) 183},                  //'' (183)  |11111111|11111111|1101111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,F,T,F}, (byte) 184},                        //'' (184)  |11111111|11111110|1010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,F,F,T,F}, (byte) 185},                    //'' (185)  |11111111|11111111|100010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,F,F,T,T}, (byte) 186},                    //'' (186)  |11111111|11111111|100011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,F,T,F,F}, (byte) 187},                    //'' (187)  |11111111|11111111|100100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,F,F,F}, (byte) 188},                  //'' (188)  |11111111|11111111|1110000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,F,T,F,T}, (byte) 189},                    //'' (189)  |11111111|11111111|100101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,F,T,T,F}, (byte) 190},                    //'' (190)  |11111111|11111111|100110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,F,F,T}, (byte) 191},                  //'' (191)  |11111111|11111111|1110001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,F,F, F,F}, (byte) 192},           //'' (192)  |11111111|11111111|11111000|00
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,F,F, F,T}, (byte) 193},           //'' (193)  |11111111|11111111|11111000|01
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,F,T,T}, (byte) 194},                        //'' (194)  |11111111|11111110|1011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,F,T}, (byte) 195},                          //'' (195)  |11111111|11111110|001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,F,T,T,T}, (byte) 196},                    //'' (196)  |11111111|11111111|100111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,F,T,F}, (byte) 197},                  //'' (197)  |11111111|11111111|1110010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,F,F,F}, (byte) 198},                    //'' (198)  |11111111|11111111|101000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,T,T,F, F}, (byte) 199},             //'' (199)  |11111111|11111111|11110110|0
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,F,F, T,F}, (byte) 200},           //'' (200)  |11111111|11111111|11111000|10
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,F,F, T,T}, (byte) 201},           //'' (201)  |11111111|11111111|11111000|11
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,F,T, F,F}, (byte) 202},           //'' (202)  |11111111|11111111|11111001|00
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,T,T, T,T,F}, (byte) 203},         //'' (203)  |11111111|11111111|11111011|110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,T,T, T,T,T}, (byte) 204},         //'' (204)  |11111111|11111111|11111011|111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,F,T, F,T}, (byte) 205},           //'' (205)  |11111111|11111111|11111001|01
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,F,F,T}, (byte) 206},                //'' (206)  |11111111|11111111|11110001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,T,T,F, T}, (byte) 207},             //'' (207)  |11111111|11111111|11110110|1
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,T,F}, (byte) 208},                          //'' (208)  |11111111|11111110|010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,F,T,T}, (byte) 209},                      //'' (209)  |11111111|11111111|00011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,F,T, T,F}, (byte) 210},           //'' (210)  |11111111|11111111|11111001|10
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,F, F,F,F}, (byte) 211},         //'' (211)  |11111111|11111111|11111100|000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,F, F,F,T}, (byte) 212},         //'' (212)  |11111111|11111111|11111100|001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,F,T, T,T}, (byte) 213},           //'' (213)  |11111111|11111111|11111001|11
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,F, F,T,F}, (byte) 214},         //'' (214)  |11111111|11111111|11111100|010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,F,T,F}, (byte) 215},                //'' (215)  |11111111|11111111|11110010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,T,F,F}, (byte) 216},                      //'' (216)  |11111111|11111111|00100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,T,F,T}, (byte) 217},                      //'' (217)  |11111111|11111111|00101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,T,F, F,F}, (byte) 218},           //'' (218)  |11111111|11111111|11111010|00
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,T,F, F,T}, (byte) 219},           //'' (219)  |11111111|11111111|11111010|01
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,F,T}, (byte) 220},       //'' (220)  |11111111|11111111|11111111|1101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,F, F,T,T}, (byte) 221},         //'' (221)  |11111111|11111111|11111100|011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,F, T,F,F}, (byte) 222},         //'' (222)  |11111111|11111111|11111100|100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,F, T,F,T}, (byte) 223},         //'' (223)  |11111111|11111111|11111100|101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,T,F,F}, (byte) 224},                        //'' (224)  |11111111|11111110|1100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,F,T,T}, (byte) 225},                //'' (225)  |11111111|11111111|11110011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, T,T,F,T}, (byte) 226},                        //'' (226)  |11111111|11111110|1101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,T,T,F}, (byte) 227},                      //'' (227)  |11111111|11111111|00110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,F,F,T}, (byte) 228},                    //'' (228)  |11111111|11111111|101001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,F,T,T,T}, (byte) 229},                      //'' (229)  |11111111|11111111|00111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, F,T,F,F,F}, (byte) 230},                      //'' (230)  |11111111|11111111|01000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,F,T,T}, (byte) 231},                  //'' (231)  |11111111|11111111|1110011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,F,T,F}, (byte) 232},                    //'' (232)  |11111111|11111111|101010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,F,T,F,T,T}, (byte) 233},                    //'' (233)  |11111111|11111111|101011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,T,T,T, F}, (byte) 234},             //'' (234)  |11111111|11111111|11110111|0
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,T,T,T, T}, (byte) 235},             //'' (235)  |11111111|11111111|11110111|1
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,T,F,F}, (byte) 236},                //'' (236)  |11111111|11111111|11110100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,F,T,F,T}, (byte) 237},                //'' (237)  |11111111|11111111|11110101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,T,F, T,F}, (byte) 238},           //'' (238)  |11111111|11111111|11111010|10
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F,T,F,F}, (byte) 239},                  //'' (239)  |11111111|11111111|1110100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,T,F, T,T}, (byte) 240},           //'' (240)  |11111111|11111111|11111010|11
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,F, T,T,F}, (byte) 241},         //'' (241)  |11111111|11111111|11111100|110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,T,T, F,F}, (byte) 242},           //'' (242)  |11111111|11111111|11111011|00
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,T,T, F,T}, (byte) 243},           //'' (243)  |11111111|11111111|11111011|01
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,F, T,T,T}, (byte) 244},         //'' (244)  |11111111|11111111|11111100|111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,T, F,F,F}, (byte) 245},         //'' (245)  |11111111|11111111|11111101|000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,T, F,F,T}, (byte) 246},         //'' (246)  |11111111|11111111|11111101|001
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,T, F,T,F}, (byte) 247},         //'' (247)  |11111111|11111111|11111101|010
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,T, F,T,T}, (byte) 248},         //'' (248)  |11111111|11111111|11111101|011
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,F}, (byte) 249},       //'' (249)  |11111111|11111111|11111111|1110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,T, T,F,F}, (byte) 250},         //'' (250)  |11111111|11111111|11111101|100
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,T, T,F,T}, (byte) 251},         //'' (251)  |11111111|11111111|11111101|101
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,T, T,T,F}, (byte) 252},         //'' (252)  |11111111|11111111|11111101|110
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,F,T, T,T,T}, (byte) 253},         //'' (253)  |11111111|11111111|11111101|111
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,F, F,F,F}, (byte) 254},         //'' (254)  |11111111|11111111|11111110|000
                {new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,F,T,T, T,F}, (byte) 255}            //'' (255)  |11111111|11111111|11111011|10
            };
        #endregion

        // see spec 07 - > Appendix C.  Huffman Codes
        // |11111111|11111111|11111111|111111
        public static readonly bool[] Eos = new[] {T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T,T,T, T,T,T,T,T,T}; 

        private const bool T = true;
        private const bool F = false;

        /// <summary>
        /// Gets the size.
        /// </summary>
        public int Size
        {
            get
            {
                Map bitsMap = _symbolBitsMap;
                return bitsMap.Keys.Sum(value => value.Length);
            }
        }

        /// <summary>
        /// Gets or sets the huffman table.
        /// </summary>
        public Map HuffmanTable
        {
            get
            {
                return _symbolBitsMap;
            }
            set
            {
                _symbolBitsMap = value;
            }
        }

        /// <summary>
        /// Get the byte.
        /// </summary>
        /// <param name="bits">The bit array.</param>
        /// <returns>The byte.</returns>
        public byte GetByte(bool[] bits)
        {
            Map bitsMap = _symbolBitsMap;
            foreach (var tableBits in bitsMap.Keys)
            {
                if (tableBits.Length != bits.Length)
                    continue;

                bool match = true;
                for (byte i = 0; i < bits.Length; i++)
                {
                    if (bits[i] != tableBits[i])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return bitsMap[tableBits];
                }
            }

            throw new Exception("Symbol is not present in the character set.");
        }

        /// <summary>
        /// Get the byte.
        /// </summary>
        /// <param name="bits">The bit array.</param>
        /// <returns>The byte.</returns>
        public byte GetByte(List<bool> bits)
        {
            Map bitsMap = _symbolBitsMap;
            foreach (var tableBits in bitsMap.Keys)
            {
                if (tableBits.Length != bits.Count)
                    continue;

                bool match = true;
                for (byte i = 0; i < bits.Count; i++)
                {
                    if (bits[i] != tableBits[i])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return bitsMap[tableBits];
                }
            }

            throw new Exception("Symbol is not present in the character set.");
        }

        /// <summary>
        /// Get the bits.
        /// </summary>
        /// <param name="c">The byte.</param>
        /// <returns>The bit collection.</returns>
        public bool[] GetBits(byte c)
        {
            var bitsMap = _symbolBitsMap;
            var val = bitsMap.FirstOrDefault(pair => pair.Value == c).Key;

            if (val == null)
                throw new Exception("Symbol is not present in the character set.");

            return val;
        }
    }
}
