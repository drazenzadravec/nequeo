<?xml version="1.0"?>

<!-- ******************************************************** -->
<!--  XSL Transform of MathML content to MathML presentation  -->
<!--             Version 1.0 RC2 from 13-Jun-2003             -->
<!--                                                          -->
<!--    Complies with the W3C MathML 2.0 Recommenation of     -->
<!--                    21 February 2001.                     -->
<!--                                                          -->
<!--   Authors Igor Rodionov <igor@csd.uwo.ca>,               -->
<!--           Stephen Watt  <watt@csd.uwo.ca>.               -->  
<!--                                                          -->
<!-- (C) Copyright 2000-2003 Symbolic Computation Laboratory, -->
<!--                         University of Western Ontario,   -->
<!--                         London, Canada N6A 5B7.          -->
<!-- ******************************************************** -->

<xsl:stylesheet id="mmlctop2.xsl"
                version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:mml="http://www.w3.org/1998/Math/MathML"
                xmlns="http://www.w3.org/1998/Math/MathML">

  <xsl:output method="xml" indent="yes"/>

  <!--<xsl:strip-space elements="apply semantics annotation-xml
        csymbol fn cn ci interval matrix matrixrow vector
        lambda bvar condition logbase degree set list
        lowlimit uplimit math"/>-->


  <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
  <!--         Parameters, variables and constants           -->
  <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->

  <!-- ~~~~~~~~ Semantics related *constants*: ~~~~~~~~ -->

  <!-- Strip off semantics -->
  <xsl:variable name="SEM_STRIP" select="-1"/>

  <!-- Pass semantics "as is" -->
  <xsl:variable name="SEM_PASS" select="0"/>

  <!-- Add semantics at top level only -->
  <xsl:variable name="SEM_TOP" select="1"/>

  <!-- Add semantics at all levels -->
  <xsl:variable name="SEM_ALL" select="2"/>

  <!-- Semantics at top level only, with id refs -->
  <!-- NOTE: ids have to be already present in the
           input for this feature to work. -->
  <xsl:variable name="SEM_XREF" select="3"/>

  <!-- No semantics at top level, with id refs -->
  <!-- NOTE: ids have to be already present in the
           input for this feature to work. -->
  <xsl:variable name="SEM_XREF_EXT" select="4"/>


  <!-- ~~~~~~~~~~ Stylesheet *parameter*: SEM_SW ~~~~~~~~~~~~~~ -->
  <!-- Assumes one of the above values; SEM_PASS is the default -->
  <!-- The default can be overridden by specifying different    -->
  <!-- value on the command line when the stylesheet is invoked -->

  <xsl:param name="SEM_SW" select="SEM_PASS"/>


  <!-- ~~~~~~ Operator precedence definitions ~~~~~~ -->

  <xsl:variable name="NO_PREC" select="0"/>
  <xsl:variable name="UNION_PREC" select="1"/>
  <xsl:variable name="SETDIFF_PREC" select="1"/>
  <xsl:variable name="INTERSECT_PREC" select="3"/>
  <xsl:variable name="CARTPROD_PREC" select="3"/>
  <xsl:variable name="OR_PREC" select="5"/>
  <xsl:variable name="XOR_PREC" select="7"/>
  <xsl:variable name="AND_PREC" select="9"/>
  <xsl:variable name="NOT_PREC" select="11"/>
  <xsl:variable name="PLUS_PREC" select="13"/>
  <xsl:variable name="MINUS_PREC" select="13"/>
  <xsl:variable name="NEG_PREC" select="15"/>
  <xsl:variable name="MUL_PREC" select="17"/>
  <xsl:variable name="DIV_PREC" select="17"/>
  <xsl:variable name="REM_PREC" select="17"/>
  <xsl:variable name="FUNCTN_PREC" select="97"/>
  <xsl:variable name="GEN_FUN_PREC" select="99"/>

  <!-- ~~~~~ Miscellaneous constant definitions ~~~~~ -->

  <xsl:variable name="YES" select="1"/>
  <xsl:variable name="NO" select="0"/>
  <xsl:variable name="NO_PARAM" select="-1"/>
  <xsl:variable name="PAR_SAME" select="-3"/>
  <xsl:variable name="PAR_YES" select="-5"/>
  <xsl:variable name="PAR_NO" select="-7"/>



  <!-- +++++++++++++++++ INDEX OF TEMPLATES +++++++++++++++++++ -->

  <!-- All templates are subdivided into the following categories
     (listed in the order of appearance in the stylesheet):

THE TOPMOST ELEMENT: MATH
 math

SEMANTICS HANDLING
 semantics

BASIC CONTAINER ELEMENTS
 cn, ci; csymbol

BASIC CONTENT ELEMENTS
 fn, interval, inverse, sep, condition, declare, lambda, compose,
 ident; domain, codomain, image, domainofapplication, piecewise

ARITHMETIC, ALGEBRA & LOGIC
 quotient, exp, factorial, max, min, minus, plus, power, rem, divide,
 times, root, gcd, and, or, xor, not, forall, exists, abs, conjugate;
 arg, real, imaginary, lcm, floor, ceiling

RELATIONS
 neq, approx, tendsto, implies, in, notin, notsubset, notprsubset,
 subset, prsubset, eq, gt, lt, geq, leq; equivalent, factorof

CALCULUS
 ln, log, diff, partialdiff, lowlimit, uplimit, bvar, degree,
 logbase; divergence, grad, curl, laplacian

SET THEORY
 set, list, union, intersect, setdiff; card, cartesianproduct

SEQUENCES AND SERIES
 sum, product, limit

TRIGONOMETRY
 sin, cos, tan, sec, csc, cot, sinh, cosh, tanh, sech, csch, coth,
 arcsin, arccos, arctan, arcsec, arccsc, arccot, arcsinh, arccosh,
 arctanh, arcsech, arccsch, arccoth

STATISTICS
 mean, sdev, variance, median, mode, moment, momentabout

LINEAR ALGEBRA
 vector, matrix, matrixrow, determinant, transpose, selector;
 vectorproduct, scalarproduct, outerproduct

CONSTANT and SYMBOL ELEMENTS
 integers, reals, rationals, naturalnumbers, complexes, primes,
 exponentiale, imaginaryi, notanumber, true, false, emptyset,
 pi, eulergamma, infinity
-->



  <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
  <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~ TEMPLATES ~~~~~~~~~~~~~~~~~~~~~~~~~ -->
  <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->



  <!-- *********************** COPY THROUGH ************************ -->

  <xsl:template match = "*">
    <xsl:copy>
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </xsl:copy>
  </xsl:template>



  <!-- ***************** THE TOPMOST ELEMENT: MATH ***************** -->

  <xsl:template match = "mml:math">
    <math>
      <xsl:copy-of select="@*"/>
      <xsl:choose>
        <xsl:when test="$SEM_SW=$SEM_TOP or $SEM_SW=$SEM_ALL and *[2] or
	                  $SEM_SW=$SEM_XREF">
          <semantics>
            <mrow>
              <xsl:apply-templates mode = "semantics"/>
            </mrow>
            <annotation-xml encoding="MathML">
              <xsl:copy-of select="*"/>
            </annotation-xml>
          </semantics>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates mode = "semantics"/>
        </xsl:otherwise>
      </xsl:choose>
    </math>
  </xsl:template>



  <!-- ***************** SEMANTICS HANDLING ***************** -->

  <!-- This template is called recursively.  At each level   -->
  <!-- in the source tree it decides whether to strip off,   -->
  <!-- pass or add semantics at that level (depending on the -->
  <!-- value of SEM_SW parameter).  Then the actual template -->
  <!-- is applied to the node.                               -->

  <xsl:template match = "mml:*" mode = "semantics">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$SEM_SW=$SEM_STRIP and self::mml:semantics">
        <xsl:apply-templates select="mml:annotation-xml[@encoding='MathML']">
          <xsl:with-param name="IN_PREC" select="$IN_PREC"/>
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:when test="($SEM_SW=$SEM_PASS or $SEM_SW=$SEM_TOP) and self::mml:semantics">
        <semantics>
          <xsl:apply-templates select="*[1]">
            <xsl:with-param name="IN_PREC" select="$IN_PREC"/>
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
          <xsl:copy-of select="mml:annotation-xml"/>
        </semantics>
      </xsl:when>
      <xsl:when test="$SEM_SW=$SEM_ALL">
        <semantics>
          <xsl:choose>
            <xsl:when test="self::mml:semantics">
              <xsl:apply-templates select="*[1]">
                <xsl:with-param name="IN_PREC" select="$IN_PREC"/>
                <xsl:with-param name="PARAM" select="$PARAM"/>
                <xsl:with-param name="PAREN" select="$PAREN"/>
                <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
              </xsl:apply-templates>
              <xsl:copy-of select="mml:annotation-xml"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:apply-templates select=".">
                <xsl:with-param name="IN_PREC" select="$IN_PREC"/>
                <xsl:with-param name="PARAM" select="$PARAM"/>
                <xsl:with-param name="PAREN" select="$PAREN"/>
                <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
              </xsl:apply-templates>
              <annotation-xml encoding="MathML">
                <xsl:copy-of select="."/>
              </annotation-xml>
            </xsl:otherwise>
          </xsl:choose>
        </semantics>
      </xsl:when>
      <xsl:when test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:choose>
          <xsl:when test="self::mml:sematics">
            <xsl:copy>
              <xsl:copy-of select="@*"/>
              <xsl:attribute name="xref">
                <xsl:value-of select="@id"/>
              </xsl:attribute>
              <xsl:copy-of select="*[1]"/>
              <xsl:copy-of select="mml:annotation-xml"/>
            </xsl:copy>
          </xsl:when>
          <xsl:otherwise>
            <xsl:apply-templates select=".">
              <xsl:with-param name="IN_PREC" select="$IN_PREC"/>
              <xsl:with-param name="PARAM" select="$PARAM"/>
              <xsl:with-param name="PAREN" select="$PAREN"/>
              <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
            </xsl:apply-templates>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select=".">
          <xsl:with-param name="IN_PREC" select="$IN_PREC"/>
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template match = "mml:semantics">
    <xsl:apply-templates select="*[1]" mode = "semantics"/>
  </xsl:template>



  <!-- ***************** BASIC CONTAINER ELEMENTS ***************** -->

  <xsl:template match = "mml:cn">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test=". &lt; 0 and $IN_PREC &gt; $NO_PREC and $PAREN=$PAR_NO
                                                   and $PAR_NO_IGNORE=$NO">
        <mfenced separators="">
          <xsl:apply-templates select="." mode="cn"/>
        </mfenced>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select="." mode="cn"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:cn" mode="cn">
    <xsl:choose>
      <xsl:when test="(not(@type) or @type='integer' or @type='real') and @base">
        <msub>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mn>
            <xsl:apply-templates mode = "semantics"/>
          </mn>
          <mn>
            <xsl:value-of select="@base"/>
          </mn>
        </msub>
      </xsl:when>
      <xsl:when test="not(@type) or @type='integer' or @type='real'">
        <mn>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates mode = "semantics"/>
        </mn>
      </xsl:when>
      <xsl:when test="@type='constant'">
        <mn>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates mode = "semantics"/>
        </mn>
      </xsl:when>
      <xsl:when test="@type='e-notation' and not(@base) and child::mml:sep[1]">
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mn>
            <xsl:apply-templates select="text()[1]" mode = "semantics"/>
          </mn>
          <mo> e </mo>
          <mn>
            <xsl:apply-templates select="text()[2]" mode = "semantics"/>
          </mn>
        </mrow>
      </xsl:when>
      <xsl:when test="@type='complex-cartesian' and not(@base) and child::mml:sep[1]">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mn>
            <xsl:apply-templates select="text()[1]" mode = "semantics"/>
          </mn>
          <xsl:if test="text()[2] &lt; 0">
            <mo> - </mo>
            <mn>
              <xsl:value-of select="-text()[2]"/>
            </mn>
          </xsl:if>
          <xsl:if test="not(text()[2] &lt; 0)">
            <mo> + </mo>
            <mn>
              <xsl:value-of select="text()[2]"/>
            </mn>
          </xsl:if>
          <mo>
            <xsl:text disable-output-escaping='yes'>&amp;#x2062;</xsl:text>
          </mo>
          <mo>
            <xsl:text disable-output-escaping='yes'>&amp;#x2148;</xsl:text>
          </mo>
        </mfenced>
      </xsl:when>
      <xsl:when test="@type='complex-cartesian' and @base and child::mml:sep[1]">
        <msub>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mfenced separators="">
            <mn>
              <xsl:apply-templates select="text()[1]"/>
            </mn>
            <xsl:if test="text()[2] &lt; 0">
              <mo> - </mo>
              <mn>
                <xsl:value-of select="-text()[2]"/>
              </mn>
            </xsl:if>
            <xsl:if test="not(text()[2] &lt; 0)">
              <mo> + </mo>
              <mn>
                <xsl:apply-templates select="text()[2]"/>
              </mn>
            </xsl:if>
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x2062;</xsl:text>
            </mo>
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x2148;</xsl:text>
            </mo>
          </mfenced>
          <mn>
            <xsl:value-of select="@base"/>
          </mn>
        </msub>
      </xsl:when>
      <xsl:when test="@type='rational' and not(@base) and child::mml:sep[1]">
        <mfrac>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mn>
            <xsl:apply-templates select="text()[1]"/>
          </mn>
          <mn>
            <xsl:apply-templates select="text()[2]"/>
          </mn>
        </mfrac>
      </xsl:when>
      <xsl:when test="@type='rational' and @base and child::mml:sep[1]">
        <msub>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mfenced>
            <mfrac>
              <mn>
                <xsl:apply-templates select="text()[1]"/>
              </mn>
              <mn>
                <xsl:apply-templates select="text()[2]"/>
              </mn>
            </mfrac>
          </mfenced>
          <mn>
            <xsl:value-of select="@base"/>
          </mn>
        </msub>
      </xsl:when>
      <xsl:when test="@type='complex-polar' and not(@base) and child::mml:sep[1]">
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mo> Polar </mo>
          <mo>
            <xsl:text disable-output-escaping='yes'>&amp;#x2062;</xsl:text>
          </mo>
          <mfenced separators=",">
            <mn>
              <xsl:apply-templates select="text()[1]"/>
            </mn>
            <mn>
              <xsl:apply-templates select="text()[2]"/>
            </mn>
          </mfenced>
        </mrow>
      </xsl:when>
      <xsl:when test="@type='complex-polar' and @base and child::mml:sep[1]">
        <msub>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mrow>
            <mo> Polar </mo>
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x2062;</xsl:text>
            </mo>
            <mfenced separators=",">
              <mn>
                <xsl:apply-templates select="text()[1]"/>
              </mn>
              <mn>
                <xsl:apply-templates select="text()[2]"/>
              </mn>
            </mfenced>
          </mrow>
          <mn>
            <xsl:value-of select="@base"/>
          </mn>
        </msub>
      </xsl:when>
      <xsl:otherwise>
        <mn>
          <xsl:apply-templates mode = "semantics"/>
        </mn>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template match = "mml:ci">
    <xsl:choose>
      <xsl:when test="@type='vector' or @type='matrix' or @type='set'">
        <mi mathvariant="bold">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates mode = "semantics"/>
        </mi>
      </xsl:when>
      <xsl:when test="child::text() and not(child::*[1])">
        <mi>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates/>
        </mi>
      </xsl:when>
      <xsl:when test="child::text() and *[1] and not(*[1]=mml:sep)">
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates/>
        </mrow>
      </xsl:when>
      <xsl:otherwise>
        <xsl:if test="*[2]">
          <mrow>
            <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
              <xsl:attribute name="xref">
                <xsl:value-of select="@id"/>
              </xsl:attribute>
            </xsl:if>
            <xsl:apply-templates select="*"/>
          </mrow>
        </xsl:if>
        <xsl:if test="not(*[2])">
          <xsl:apply-templates select="*[1]"/>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:ci/mml:*[not(self::mml:sep)]">
    <xsl:copy-of select = "."/>
  </xsl:template>


  <xsl:template match = "mml:csymbol">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:copy-of select = "* | text()"/>
    </mrow>
  </xsl:template>



  <!-- ***************** BASIC CONTENT ELEMENTS ***************** -->

  <!-- General <apply> <AnyFunction/> ... </apply> -->
  <!-- Dependants: csymbol apply[fn inverse compose] -->
  <xsl:template match = "mml:apply">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select = "*[1]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
        <xsl:with-param name="PARAM" select="$PAR_SAME"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
      </mo>
      <mfenced separators=",">
        <xsl:apply-templates select = "*[position()>1]" mode = "semantics">
          <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
          <xsl:with-param name="PARAM" select="$PAR_SAME"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
        </xsl:apply-templates>
      </mfenced>
    </mrow>
  </xsl:template>


  <!-- mml:fn is ***DEPRECATED*** -->
  <xsl:template match = "mml:fn">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:apply-templates mode = "semantics">
      <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
      <xsl:with-param name="PARAM" select="$PAR_SAME"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
    </xsl:apply-templates>
  </xsl:template>


  <xsl:template match = "mml:interval">
    <mfenced>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:if test="not(@closure) or @closure='closed' or @closure='closed-open' or not(@closure='open') and not(@closure='open-closed')">
        <xsl:attribute name="open"> [ </xsl:attribute>
      </xsl:if>
      <xsl:if test="not(@closure) or @closure='closed' or @closure='open-closed' or not(@closure='open') and not(@closure='closed-open')">
        <xsl:attribute name="close"> ] </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="*" mode = "semantics"/>
    </mfenced>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:*[1][self::mml:inverse]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="*[2]=mml:exp | *[2]=mml:ln | *[2]=mml:sin | *[2]=mml:cos |
                    *[2]=mml:tan | *[2]=mml:sec | *[2]=mml:csc | *[2]=mml:cot |
                    *[2]=mml:sinh | *[2]=mml:cosh | *[2]=mml:tanh | *[2]=mml:sech |
                    *[2]=mml:csch | *[2]=mml:coth | *[2]=mml:arcsin |
                    *[2]=mml:arccos | *[2]=mml:arctan">
        <mo>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="*[2]" mode="inverse"/>
        </mo>
      </xsl:when>
      <xsl:otherwise>
        <msup>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mrow>
            <xsl:apply-templates select = "*[2]">
              <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
              <xsl:with-param name="PARAM" select="$PAR_SAME"/>
              <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
            </xsl:apply-templates>
          </mrow>
          <mfenced>
            <mn> -1 </mn>
          </mfenced>
        </msup>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "*" mode="inverse">
    <xsl:choose>
      <xsl:when test="self::mml:exp">
        <xsl:value-of select="'ln'"/>
      </xsl:when>
      <xsl:when test="self::mml:ln">
        <xsl:value-of select="'exp'"/>
      </xsl:when>
      <xsl:when test="self::mml:sin">
        <xsl:value-of select="'arcsin'"/>
      </xsl:when>
      <xsl:when test="self::mml:cos">
        <xsl:value-of select="'arccos'"/>
      </xsl:when>
      <xsl:when test="self::mml:tan">
        <xsl:value-of select="'arctan'"/>
      </xsl:when>
      <xsl:when test="self::mml:sec">
        <xsl:value-of select="'arcsec'"/>
      </xsl:when>
      <xsl:when test="self::mml:csc">
        <xsl:value-of select="'arccsc'"/>
      </xsl:when>
      <xsl:when test="self::mml:cot">
        <xsl:value-of select="'arccot'"/>
      </xsl:when>
      <xsl:when test="self::mml:sinh">
        <xsl:value-of select="'arcsinh'"/>
      </xsl:when>
      <xsl:when test="self::mml:cosh">
        <xsl:value-of select="'arccosh'"/>
      </xsl:when>
      <xsl:when test="self::mml:tanh">
        <xsl:value-of select="'arctanh'"/>
      </xsl:when>
      <xsl:when test="self::mml:sech">
        <xsl:value-of select="'arcsech'"/>
      </xsl:when>
      <xsl:when test="self::mml:csch">
        <xsl:value-of select="'arccsch'"/>
      </xsl:when>
      <xsl:when test="self::mml:coth">
        <xsl:value-of select="'arccoth'"/>
      </xsl:when>
      <xsl:when test="self::mml:arcsin">
        <xsl:value-of select="'sin'"/>
      </xsl:when>
      <xsl:when test="self::mml:arccos">
        <xsl:value-of select="'cos'"/>
      </xsl:when>
      <xsl:when test="self::mml:arctan">
        <xsl:value-of select="'tan'"/>
      </xsl:when>
    </xsl:choose>
  </xsl:template>


  <xsl:template match = "mml:condition">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="*" mode = "semantics"/>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:declare"/>


  <xsl:template match = "mml:lambda">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x03BB;</xsl:text>
      </mo>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
      </mo>
      <mfenced>
        <xsl:for-each select = "*">
          <xsl:choose>
            <xsl:when test="self::mml:ci or self::mml:cn">
              <xsl:apply-templates select = "." mode="semantics"/>
            </xsl:when>
            <xsl:otherwise>
              <mrow>
                <xsl:apply-templates select = "." mode="semantics"/>
              </mrow>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </mfenced>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[*[1][self::mml:compose]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &lt; $GEN_FUN_PREC and
                   ($IN_PREC &gt; $FUNCTN_PREC or $IN_PREC=$FUNCTN_PREC and $PARAM=$PAR_SAME)">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select = "*[2]" mode="semantics"/>
          <xsl:for-each select = "mml:*[position()>2]">
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x2218;</xsl:text>
            </mo>
            <xsl:apply-templates select = "." mode="semantics">
              <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
              <xsl:with-param name="PARAM" select="$PAR_SAME"/>
              <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
            </xsl:apply-templates>
          </xsl:for-each>
        </mfenced>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select = "*[2]" mode="semantics"/>
          <xsl:for-each select = "*[position()>2]">
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x2218;</xsl:text>
            </mo>
            <xsl:apply-templates select = "." mode="semantics">
              <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
              <xsl:with-param name="PARAM" select="$PAR_SAME"/>
              <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
            </xsl:apply-templates>
          </xsl:for-each>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template match = "mml:ident">
    <xsl:choose>
      <xsl:when test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <mtext xref="{@id}">id</mtext>
      </xsl:when>
      <xsl:otherwise>
        <mtext>id</mtext>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template match="mml:apply[*[1]=mml:domain or *[1]=mml:codomain or *[1]=mml:image]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:if test="*[1]=mml:domain">
        <mtext>domain</mtext>
      </xsl:if>
      <xsl:if test="*[1]=mml:codomain">
        <mtext>codomain</mtext>
      </xsl:if>
      <xsl:if test="*[1]=mml:image">
        <mtext>image</mtext>
      </xsl:if>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
      </mo>
      <mfenced separators="">
        <xsl:apply-templates select="*[position()>1]" mode = "semantics"/>
      </mfenced>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:domainofapplication">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select = "*" mode = "semantics"/>
    </mrow>
  </xsl:template>


  <xsl:template match="mml:piecewise">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo stretchy="true"> { </mo>
      <mtable columnalign="left left">
        <xsl:for-each select="mml:piece">
          <mtr>
            <mtd>
              <xsl:apply-templates select="*[position()=1]" mode = "semantics"/>
            </mtd>
            <mtd>
              <mtext>if </mtext>
              <xsl:apply-templates select="*[position()=2]" mode = "semantics"/>
            </mtd>
          </mtr>
        </xsl:for-each>
        <xsl:if test="mml:otherwise">
          <mtr>
            <mtd>
              <xsl:apply-templates select="mml:otherwise/*" mode = "semantics"/>
            </mtd>
            <mtd>
              <mtext>otherwise</mtext>
            </mtd>
          </mtr>
        </xsl:if>
      </mtable>
    </mrow>
  </xsl:template>



  <!-- ***************** ARITHMETIC, ALGEBRA & LOGIC ***************** -->

  <xsl:template match = "mml:apply[mml:quotient[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x230A;</xsl:text>
      </mo>
      <mfrac>
        <mrow>
          <xsl:apply-templates select="*[2]" mode = "semantics">
            <xsl:with-param name="IN_PREC" select="$DIV_PREC"/>
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
        <mrow>
          <xsl:apply-templates select="*[3]" mode = "semantics">
            <xsl:with-param name="IN_PREC" select="$DIV_PREC"/>
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </mfrac>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x230B;</xsl:text>
      </mo>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[*[1][self::mml:exp]]">
    <msup>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mn>
        <xsl:text disable-output-escaping='yes'>&amp;#x2147;</xsl:text>
      </mn>
      <xsl:apply-templates select = "*[2]" mode = "semantics"/>
    </msup>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:factorial[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select = "*[2]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
      <mo> ! </mo>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:max[1] | mml:min[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="*[2]=mml:bvar">
          <munder>
            <xsl:if test="*[1]=mml:max">
              <mo> max </mo>
            </xsl:if>
            <xsl:if test="*[1]=mml:min">
              <mo> min </mo>
            </xsl:if>
            <xsl:apply-templates select="*[2]" mode = "semantics"/>
          </munder>
        </xsl:when>
        <xsl:otherwise>
          <xsl:if test="*[1]=mml:max">
            <mo> max </mo>
          </xsl:if>
          <xsl:if test="*[1]=mml:min">
            <mo> min </mo>
          </xsl:if>
        </xsl:otherwise>
      </xsl:choose>
      <mfenced open="{{" close="}}">
        <xsl:if test="child::mml:condition">
          <xsl:attribute name="separators"/>
          <xsl:if test="*[position()>1 and not(self::mml:bvar) and not(self::mml:condition)]">
            <mfenced open="" close="" separators=",">
              <xsl:for-each select = "*[position()>1 and not(self::mml:bvar) and not(self::mml:condition)]">
                <xsl:apply-templates select = "." mode="semantics"/>
              </xsl:for-each>
            </mfenced>
            <mo lspace="0.1666em" rspace="0.1666em"> | </mo>
          </xsl:if>
          <xsl:apply-templates select="mml:condition" mode = "semantics"/>
        </xsl:if>
        <xsl:if test="not(child::mml:condition)">
          <xsl:for-each select = "*[position()>1 and not(self::mml:bvar)]">
            <xsl:apply-templates select = "." mode="semantics"/>
          </xsl:for-each>
        </xsl:if>
      </mfenced>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:minus[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &lt; $GEN_FUN_PREC and
                   ($IN_PREC &gt; $MINUS_PREC or $IN_PREC=$MINUS_PREC and $PARAM=$PAR_SAME)">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="minus">
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAR_YES"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="minus">
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="minus">
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:apply[mml:minus[1]]" mode="minus">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:if test="not(*[3])">
      <mo> - </mo>
      <xsl:apply-templates select="*[2]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$NEG_PREC"/>
        <xsl:with-param name="PAREN" select="$PAREN"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </xsl:if>
    <xsl:if test="*[3]">
      <xsl:apply-templates select="*[2]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$MINUS_PREC"/>
        <xsl:with-param name="PARAM" select="$PARAM"/>
        <xsl:with-param name="PAREN" select="$PAREN"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
      </xsl:apply-templates>
      <mo> - </mo>
      <xsl:apply-templates select="*[3]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$MINUS_PREC"/>
        <xsl:with-param name="PARAM" select="$PAR_SAME"/>
        <xsl:with-param name="PAREN" select="$PAREN"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </xsl:if>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:plus[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &lt; $GEN_FUN_PREC and
                   ($IN_PREC &gt; $PLUS_PREC or $IN_PREC=$PLUS_PREC and $PARAM=$PAR_SAME)">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="plus">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAR_YES"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="plus">
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="plus">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:apply[mml:plus[1]]" mode="plus">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:if test="*[2]">
      <xsl:apply-templates select="*[2]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$PLUS_PREC"/>
        <xsl:with-param name="PARAM" select="$PARAM"/>
        <xsl:with-param name="PAREN" select="$PAREN"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
      </xsl:apply-templates>
      <xsl:for-each select = "*[position()>2]">
        <xsl:choose>
          <xsl:when test=". &lt; 0">
            <mo> - </mo>
            <mn>
              <xsl:value-of select="-."/>
            </mn>
          </xsl:when>
          <xsl:when test="self::mml:apply[mml:minus[1]] and not(*[3])">
            <xsl:apply-templates select="." mode = "semantics">
              <xsl:with-param name="IN_PREC" select="$PLUS_PREC"/>
              <xsl:with-param name="PAREN" select="$PAREN"/>
              <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
            </xsl:apply-templates>
          </xsl:when>
          <xsl:otherwise>
            <mo> + </mo>
            <xsl:apply-templates select="." mode = "semantics">
              <xsl:with-param name="IN_PREC" select="$PLUS_PREC"/>
              <xsl:with-param name="PAREN" select="$PAREN"/>
              <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
            </xsl:apply-templates>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>


  <xsl:template match = "mml:apply[*[1][self::mml:power]]">
    <xsl:choose>
      <xsl:when test="*[2]=mml:apply[mml:ln[1] | mml:log[1] | mml:abs[1] |
                         mml:gcd[1] | mml:lcm[1] | mml:sin[1] | mml:cos[1] | mml:tan[1] |
                         mml:sec[1] | mml:csc[1] | mml:cot[1] | mml:sinh[1] |
                         mml:cosh[1] | mml:tanh[1] | mml:sech[1] | mml:csch[1] |
                         mml:coth[1] | mml:arcsin[1] | mml:arccos[1] |
                         mml:arctan[1] | mml:arcsec[1] | mml:arccsc[1] |
                         mml:arccot[1] | mml:arcsinh[1] | mml:arccosh[1] |
                         mml:arctanh[1] | mml:arcsech[1] | mml:arccsch [1] |
                         mml:arccoth[1]]">
        <xsl:apply-templates select="*[2]" mode = "semantics"/>
      </xsl:when>
      <xsl:otherwise>
        <msup>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select = "*[2]" mode = "semantics">
            <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
          </xsl:apply-templates>
          <xsl:apply-templates select = "*[3]" mode = "semantics"/>
        </msup>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:divide[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &lt; $GEN_FUN_PREC and
                   ($IN_PREC &gt; $DIV_PREC or $IN_PREC=$DIV_PREC and $PARAM=$PAR_SAME)">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="div">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAR_YES"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="div">
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="div">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:apply[mml:divide[1]]" mode="div">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <mfrac>
      <mrow>
        <xsl:apply-templates select = "*[2]" mode = "semantics">
          <xsl:with-param name="IN_PREC" select="$GEN_FUN_PREC"/>
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </mrow>
      <mrow>
        <xsl:apply-templates select = "*[3]" mode = "semantics">
          <xsl:with-param name="IN_PREC" select="$GEN_FUN_PREC"/>
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </mrow>
    </mfrac>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:rem[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &lt; $GEN_FUN_PREC and
                   ($IN_PREC &gt; $REM_PREC or $IN_PREC=$REM_PREC and $PARAM=$PAR_SAME)">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="rem">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAR_YES"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="rem">
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="rem">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:apply[mml:rem[1]]" mode="rem">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:apply-templates select = "*[2]" mode = "semantics">
      <xsl:with-param name="IN_PREC" select="$REM_PREC"/>
      <xsl:with-param name="PARAM" select="$PARAM"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
    </xsl:apply-templates>
    <mo lspace="thickmathspace" rspace="thickmathspace">
      <xsl:value-of select="'mod'"/>
    </mo>
    <xsl:apply-templates select = "*[3]" mode = "semantics">
      <xsl:with-param name="IN_PREC" select="$REM_PREC"/>
      <xsl:with-param name="PARAM" select="$PAR_SAME"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
    </xsl:apply-templates>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:times[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &lt; $GEN_FUN_PREC and
                   ($IN_PREC &gt; $MUL_PREC or $IN_PREC=$MUL_PREC and $PARAM=$PAR_SAME)">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="times">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAR_YES"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="times">
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="times">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:apply[mml:times[1]]" mode="times">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:apply-templates select="*[2]" mode = "semantics">
      <xsl:with-param name="IN_PREC" select="$MUL_PREC"/>
      <xsl:with-param name="PARAM" select="$PARAM"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
    </xsl:apply-templates>
    <xsl:if test="*[3]">
      <xsl:for-each select = "*[position()>2]">
        <mo>
          <xsl:text disable-output-escaping='yes'>&amp;#x2062;</xsl:text>
        </mo>
        <xsl:apply-templates select="." mode = "semantics">
          <xsl:with-param name="IN_PREC" select="$MUL_PREC"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
        </xsl:apply-templates>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>


  <xsl:template match = "mml:apply[*[1]=mml:root and *[2]=mml:degree]">
    <mroot>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="*[3]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$GEN_FUN_PREC"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
      <xsl:apply-templates select="*[2]" mode = "semantics"/>
    </mroot>
  </xsl:template>

  <xsl:template match = "mml:apply[*[1]=mml:root and not(*[2]=mml:degree)]">
    <msqrt>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="*[2]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$GEN_FUN_PREC"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </msqrt>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:gcd[1] | mml:lcm[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:if test="not(parent::mml:apply[mml:power[1]])">
        <xsl:if test="mml:gcd[1]">
          <mo> gcd </mo>
        </xsl:if>
        <xsl:if test="mml:lcm[1]">
          <mo> lcm </mo>
        </xsl:if>
      </xsl:if>
      <xsl:if test="parent::mml:apply[mml:power[1]]">
        <msup>
          <xsl:if test="mml:gcd[1]">
            <mo> gcd </mo>
          </xsl:if>
          <xsl:if test="mml:lcm[1]">
            <mo> lcm </mo>
          </xsl:if>
          <xsl:apply-templates select = "../*[3]" mode = "semantics"/>
        </msup>
      </xsl:if>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
      </mo>
      <mfenced separators=",">
        <xsl:for-each select = "*[position()>1]">
          <xsl:apply-templates select = "." mode="semantics"/>
        </xsl:for-each>
      </mfenced>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:and[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &lt; $GEN_FUN_PREC and
                   ($IN_PREC &gt; $AND_PREC or $IN_PREC=$AND_PREC and $PARAM=$PAR_SAME)">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="and">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAR_YES"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="and">
          <xsl:with-param name="PARAM" select="$IN_PREC"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="and">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:apply[mml:and[1]]" mode="and">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:apply-templates select="*[2]" mode = "semantics">
      <xsl:with-param name="IN_PREC" select="$AND_PREC"/>
      <xsl:with-param name="PARAM" select="$PARAM"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
    </xsl:apply-templates>
    <xsl:for-each select = "*[position()>2]">
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2227;</xsl:text>
      </mo>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
      </mo>
      <xsl:apply-templates select="." mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$AND_PREC"/>
        <xsl:with-param name="PAREN" select="$PAREN"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </xsl:for-each>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:or[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &lt; $GEN_FUN_PREC and
                   ($IN_PREC &gt; $OR_PREC or $IN_PREC=$OR_PREC and $PARAM=$PAR_SAME)">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="or">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAR_YES"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="or">
          <xsl:with-param name="PARAM" select="$IN_PREC"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="or">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:apply[mml:or[1]]" mode="or">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:apply-templates select="*[2]" mode = "semantics">
      <xsl:with-param name="IN_PREC" select="$OR_PREC"/>
      <xsl:with-param name="PARAM" select="$PARAM"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
    </xsl:apply-templates>
    <xsl:for-each select = "*[position()>2]">
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2228;</xsl:text>
      </mo>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
      </mo>
      <xsl:apply-templates select="." mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$OR_PREC"/>
        <xsl:with-param name="PAREN" select="$PAREN"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </xsl:for-each>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:xor[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &lt; $GEN_FUN_PREC and
                   ($IN_PREC &gt; $XOR_PREC or $IN_PREC=$XOR_PREC and $PARAM=$PAR_SAME)">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="xor">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAR_YES"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                                                and not($SEM_SW=$SEM_ALL)">
        <xsl:apply-templates select="." mode="xor">
          <xsl:with-param name="PARAM" select="$IN_PREC"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="xor">
            <xsl:with-param name="PARAM" select="$IN_PREC"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:apply[mml:xor[1]]" mode="xor">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:apply-templates select="*[2]" mode = "semantics">
      <xsl:with-param name="IN_PREC" select="$XOR_PREC"/>
      <xsl:with-param name="PARAM" select="$PARAM"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
    </xsl:apply-templates>
    <xsl:for-each select = "*[position()>2]">
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x22BB;</xsl:text>
      </mo>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
      </mo>
      <xsl:apply-templates select="." mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$XOR_PREC"/>
        <xsl:with-param name="PAREN" select="$PAREN"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </xsl:for-each>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:not[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &lt; $GEN_FUN_PREC and $IN_PREC &gt;= $NOT_PREC">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mo>
            <xsl:text disable-output-escaping='yes'>&amp;#x00AC;</xsl:text>
          </mo>
          <mo>
            <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
          </mo>
          <xsl:apply-templates select = "*[2]" mode = "semantics">
            <xsl:with-param name="IN_PREC" select="$NOT_PREC"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <mo>
            <xsl:text disable-output-escaping='yes'>&amp;#x00AC;</xsl:text>
          </mo>
          <mo>
            <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
          </mo>
          <xsl:apply-templates select = "*[2]" mode = "semantics">
            <xsl:with-param name="IN_PREC" select="$NOT_PREC"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:forall[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2200;</xsl:text>
      </mo>
      <xsl:if test="count(mml:bvar)=1">
        <xsl:apply-templates select = "mml:bvar" mode="semantics"/>
      </xsl:if>
      <xsl:if test="count(mml:bvar) &gt; 1">
        <mfenced open="" close="">
          <xsl:for-each select = "mml:bvar">
            <xsl:apply-templates select = "." mode="semantics"/>
          </xsl:for-each>
        </mfenced>
      </xsl:if>
      <xsl:if test="mml:condition">
        <mo> : </mo>
        <xsl:apply-templates select = "mml:condition/*" mode = "semantics"/>
      </xsl:if>
      <xsl:if test="*[position()>1 and not(self::mml:bvar) and not(self::mml:condition)]">
        <mo> , </mo>
        <xsl:apply-templates select = "*[position()>1 and not(self::mml:bvar) and
                                not(self::mml:condition)]" mode = "semantics"/>
      </xsl:if>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:exists[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2203;</xsl:text>
      </mo>
      <xsl:if test="count(mml:bvar) &gt; 1">
        <mfenced open="" close="">
          <xsl:for-each select = "mml:bvar">
            <xsl:apply-templates select = "." mode="semantics"/>
          </xsl:for-each>
        </mfenced>
      </xsl:if>
      <xsl:if test="count(mml:bvar)=1">
        <xsl:apply-templates select = "mml:bvar" mode="semantics"/>
      </xsl:if>
      <xsl:if test="mml:condition">
        <mo> : </mo>
        <xsl:apply-templates select = "mml:condition/*" mode = "semantics"/>
      </xsl:if>
      <xsl:if test="*[position()>1 and not(self::mml:bvar) and not(self::mml:condition)]">
        <mo> , </mo>
        <xsl:apply-templates select = "*[position()>1 and not(self::mml:bvar) and
                                not(self::mml:condition)]" mode = "semantics"/>
      </xsl:if>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:abs[1]]">
    <xsl:if test="not(parent::mml:apply[mml:power[1]])">
      <mfenced open="&#x2223;" close="&#x2223;" separators="">
        <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
          <xsl:attribute name="xref">
            <xsl:value-of select="@id"/>
          </xsl:attribute>
        </xsl:if>
        <xsl:apply-templates select = "*[position()>1]" mode = "semantics"/>
      </mfenced>
    </xsl:if>
    <xsl:if test="parent::mml:apply[mml:power[1]]">
      <msup>
        <mfenced open="&#x2223;" close="&#x2223;" separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select = "*[position()>1]" mode = "semantics"/>
        </mfenced>
        <xsl:apply-templates select = "../*[3]" mode = "semantics"/>
      </msup>
    </xsl:if>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:conjugate[1]]">
    <mover>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mrow>
        <xsl:apply-templates select = "*[position()>1]" mode = "semantics"/>
      </mrow>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x00AF;</xsl:text>
      </mo>
    </mover>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:arg[1] | mml:real[1] | mml:imaginary[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo>
        <xsl:if test="mml:arg">
          <xsl:value-of select="'arg'"/>
        </xsl:if>
        <xsl:if test="mml:real">
          <xsl:text disable-output-escaping='yes'>&amp;#x211C;</xsl:text>
        </xsl:if>
        <xsl:if test="mml:imaginary">
          <xsl:text disable-output-escaping='yes'>&amp;#x2111;</xsl:text>
        </xsl:if>
      </mo>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
      </mo>
      <mfenced separators="">
        <xsl:apply-templates select = "*[2]" mode = "semantics"/>
      </mfenced>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:floor[1] or mml:ceiling[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo>
        <xsl:if test="mml:floor[1]">
          <xsl:text disable-output-escaping='yes'>&amp;#x230A;</xsl:text>
        </xsl:if>
        <xsl:if test="mml:ceiling[1]">
          <xsl:text disable-output-escaping='yes'>&amp;#x2308;</xsl:text>
        </xsl:if>
      </mo>
      <xsl:apply-templates select="*[position()>1]"  mode="semantics"/>
      <mo>
        <xsl:if test="mml:floor[1]">
          <xsl:text disable-output-escaping='yes'>&amp;#x230B;</xsl:text>
        </xsl:if>
        <xsl:if test="mml:ceiling[1]">
          <xsl:text disable-output-escaping='yes'>&amp;#x2309;</xsl:text>
        </xsl:if>
      </mo>
    </mrow>
  </xsl:template>



  <!-- ***************** RELATIONS ***************** -->

  <xsl:template match = "mml:apply[mml:neq | mml:approx | mml:tendsto | mml:implies
                     | mml:in | mml:notin | mml:notsubset | mml:notprsubset
                     | mml:subset | mml:prsubset | mml:eq | mml:gt | mml:lt
                     | mml:geq | mml:leq | mml:equivalent | mml:factorof]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="."  mode="relations"/>
    </mrow>
  </xsl:template>

  <!-- mml:reln is ***DEPRECATED*** -->
  <xsl:template match = "mml:reln[mml:neq | mml:approx | mml:tendsto | mml:implies
                     | mml:in | mml:notin | mml:notsubset | mml:notprsubset
                     | mml:subset | mml:prsubset | mml:eq | mml:gt | mml:lt
                     | mml:geq | mml:leq | mml:equivalent | mml:factorof]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="."  mode="relations"/>
    </mrow>
  </xsl:template>

  <xsl:template match = "*" mode="relations">
    <xsl:if test="*[1]=mml:neq or *[1]=mml:approx or *[1]=mml:factorof or *[1]=mml:tendsto or
                *[1]=mml:implies or *[1]=mml:in or *[1]=mml:notin or
                *[1]=mml:notsubset or *[1]=mml:notprsubset">
      <xsl:apply-templates select = "*[2]" mode = "semantics"/>
      <mo>
        <xsl:if test="*[1]=mml:neq">
          <xsl:text disable-output-escaping='yes'>&amp;#x2260;</xsl:text>
        </xsl:if>
        <xsl:if test="*[1]=mml:approx">
          <xsl:text disable-output-escaping='yes'>&amp;#x2248;</xsl:text>
        </xsl:if>
        <xsl:if test="*[1]=mml:factorof">
          <xsl:text disable-output-escaping='yes'>&amp;#x2223;</xsl:text>
        </xsl:if>
        <xsl:if test="*[1]=mml:tendsto">
          <xsl:choose>
            <xsl:when test="mml:tendsto[@type='above']">
              <xsl:text disable-output-escaping='yes'>&amp;#x2198;</xsl:text>
            </xsl:when>
            <xsl:when test="mml:tendsto[@type='below']">
              <xsl:text disable-output-escaping='yes'>&amp;#x2197;</xsl:text>
            </xsl:when>
            <xsl:otherwise>
              <xsl:text disable-output-escaping='yes'>&amp;#x2192;</xsl:text>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:if>
        <xsl:if test="*[1]=mml:implies">
          <xsl:text disable-output-escaping='yes'>&amp;#x21D2;</xsl:text>
        </xsl:if>
        <xsl:if test="*[1]=mml:in">
          <xsl:text disable-output-escaping='yes'>&amp;#x2208;</xsl:text>
        </xsl:if>
        <xsl:if test="*[1]=mml:notin">
          <xsl:text disable-output-escaping='yes'>&amp;#x2209;</xsl:text>
        </xsl:if>
        <xsl:if test="*[1]=mml:notsubset">
          <xsl:text disable-output-escaping='yes'>&amp;#x2284;</xsl:text>
        </xsl:if>
        <xsl:if test="*[1]=mml:notprsubset">
          <xsl:text disable-output-escaping='yes'>&amp;#x2288;</xsl:text>
        </xsl:if>
      </mo>
      <xsl:apply-templates select = "*[3]" mode = "semantics"/>
    </xsl:if>
    <xsl:if test="*[1]=mml:subset or *[1]=mml:prsubset or *[1]=mml:eq or *[1]=mml:gt
             or *[1]=mml:lt or *[1]=mml:geq or *[1]=mml:leq or *[1]=mml:equivalent">
      <xsl:apply-templates select = "*[2]" mode="semantics"/>
      <xsl:for-each select = "*[position()>2]">
        <mo>
          <xsl:if test="../*[self::mml:subset][1]">
            <xsl:text disable-output-escaping='yes'>&amp;#x2286;</xsl:text>
          </xsl:if>
          <xsl:if test="../*[self::mml:prsubset][1]">
            <xsl:text disable-output-escaping='yes'>&amp;#x2282;</xsl:text>
          </xsl:if>
          <xsl:if test="../*[self::mml:eq][1]">
            <xsl:value-of select="'='"/>
          </xsl:if>
          <xsl:if test="../*[self::mml:gt][1]">
            <xsl:value-of select="'&gt;'"/>
          </xsl:if>
          <xsl:if test="../*[self::mml:lt][1]">
            <xsl:value-of select="'&lt;'"/>
          </xsl:if>
          <xsl:if test="../*[self::mml:geq][1]">
            <xsl:text disable-output-escaping='yes'>&amp;#x2265;</xsl:text>
          </xsl:if>
          <xsl:if test="../*[self::mml:leq][1]">
            <xsl:text disable-output-escaping='yes'>&amp;#x2264;</xsl:text>
          </xsl:if>
          <xsl:if test="../*[self::mml:equivalent][1]">
            <xsl:text disable-output-escaping='yes'>&amp;#x2261;</xsl:text>
          </xsl:if>
        </mo>
        <xsl:apply-templates select = "." mode="semantics"/>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>



  <!-- ***************** CALCULUS ***************** -->

  <xsl:template match = "mml:apply[*[1][self::mml:ln]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="parent::mml:apply[mml:power[1]]">
          <msup>
            <mo> ln </mo>
            <xsl:apply-templates select = "../*[3]" mode = "semantics"/>
          </msup>
        </xsl:when>
        <xsl:otherwise>
          <mo rspace="thinmathspace"> ln </mo>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:apply-templates select = "*[2]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:log[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="parent::mml:apply[mml:power[1]]">
          <xsl:if test="not(*[2]=mml:logbase)">
            <msup>
              <mo> log </mo>
              <xsl:apply-templates select = "../*[3]" mode = "semantics"/>
            </msup>
          </xsl:if>
          <xsl:if test="*[2]=mml:logbase">
            <msubsup>
              <mo> log </mo>
              <xsl:apply-templates select = "../*[3]" mode = "semantics"/>
              <xsl:apply-templates select = "mml:logbase" mode = "semantics"/>
            </msubsup>
          </xsl:if>
        </xsl:when>
        <xsl:otherwise>
          <xsl:if test="not(*[2]=mml:logbase)">
            <mo rspace="thinmathspace"> log </mo>
          </xsl:if>
          <xsl:if test="*[2]=mml:logbase">
            <msub>
              <mo> log </mo>
              <xsl:apply-templates select = "mml:logbase" mode = "semantics"/>
            </msub>
          </xsl:if>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:if test="*[2]=mml:logbase">
        <xsl:apply-templates select = "*[3]" mode = "semantics">
          <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
        </xsl:apply-templates>
      </xsl:if>
      <xsl:if test="not(*[2]=mml:logbase)">
        <xsl:apply-templates select = "*[2]" mode = "semantics">
          <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
        </xsl:apply-templates>
      </xsl:if>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:diff[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="mml:bvar">
          <xsl:if test="not(mml:bvar[*[2]=mml:degree])">
            <mfrac>
              <mo>
                <xsl:text disable-output-escaping='yes'>&amp;#x2146;</xsl:text>
              </mo>
              <mrow>
                <mo>
                  <xsl:text disable-output-escaping='yes'>&amp;#x2146;</xsl:text>
                </mo>
                <xsl:apply-templates select = "mml:bvar/*[1]" mode = "semantics"/>
              </mrow>
            </mfrac>
          </xsl:if>
          <xsl:if test="mml:bvar[*[2]=mml:degree]">
            <mfrac>
              <msup>
                <mo>
                  <xsl:text disable-output-escaping='yes'>&amp;#x2146;</xsl:text>
                </mo>
                <xsl:apply-templates select = "mml:bvar/mml:degree" mode = "semantics"/>
              </msup>
              <mrow>
                <mo>
                  <xsl:text disable-output-escaping='yes'>&amp;#x2146;</xsl:text>
                </mo>
                <msup>
                  <xsl:apply-templates select = "mml:bvar/*[1]" mode = "semantics"/>
                  <xsl:apply-templates select = "mml:bvar/mml:degree" mode = "semantics"/>
                </msup>
              </mrow>
            </mfrac>
          </xsl:if>
          <xsl:apply-templates select = "*[position()=last() and not(self::bvar)]" mode = "semantics">
            <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
          </xsl:apply-templates>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates select = "*[position()=last() and not(self::bvar)]" mode = "semantics">
            <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
          </xsl:apply-templates>
          <mo>
            <xsl:text disable-output-escaping='yes'>&amp;#x2032;</xsl:text>
          </mo>
        </xsl:otherwise>
      </xsl:choose>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:partialdiff[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="mml:list">
          <msub>
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x2145;</xsl:text>
            </mo>
            <xsl:apply-templates select = "mml:list" mode = "semantics"/>
          </msub>
        </xsl:when>
        <xsl:otherwise>
          <xsl:if test="mml:degree">
            <mfrac>
              <msup>
                <mo>
                  <xsl:text disable-output-escaping='yes'>&amp;#x2202;</xsl:text>
                </mo>
                <xsl:apply-templates select = "mml:degree" mode = "semantics"/>
              </msup>
              <mrow>
                <xsl:for-each select = "mml:bvar">
                  <mo>
                    <xsl:text disable-output-escaping='yes'>&amp;#x2202;</xsl:text>
                  </mo>
                  <xsl:if test="*[last()]=mml:degree">
                    <msup>
                      <xsl:apply-templates select = "*[1]" mode = "semantics"/>
                      <xsl:apply-templates select = "mml:degree" mode = "semantics"/>
                    </msup>
                  </xsl:if>
                  <xsl:if test="not(*[last()]=mml:degree)">
                    <xsl:apply-templates select = "*[1]" mode = "semantics"/>
                  </xsl:if>
                </xsl:for-each>
              </mrow>
            </mfrac>
          </xsl:if>
          <xsl:if test="not(mml:degree)">
            <xsl:for-each select = "mml:bvar">
              <xsl:if test="*[last()]=mml:degree">
                <mfrac>
                  <msup>
                    <mo>
                      <xsl:text disable-output-escaping='yes'>&amp;#x2202;</xsl:text>
                    </mo>
                    <xsl:apply-templates select = "mml:degree" mode = "semantics"/>
                  </msup>
                  <mrow>
                    <mo>
                      <xsl:text disable-output-escaping='yes'>&amp;#x2202;</xsl:text>
                    </mo>
                    <msup>
                      <xsl:apply-templates select = "*[1]" mode = "semantics"/>
                      <xsl:apply-templates select = "mml:degree" mode = "semantics"/>
                    </msup>
                  </mrow>
                </mfrac>
              </xsl:if>
              <xsl:if test="not(*[last()]=mml:degree)">
                <mfrac>
                  <mo>
                    <xsl:text disable-output-escaping='yes'>&amp;#x2202;</xsl:text>
                  </mo>
                  <mrow>
                    <mo>
                      <xsl:text disable-output-escaping='yes'>&amp;#x2202;</xsl:text>
                    </mo>
                    <xsl:apply-templates select = "*[1]" mode = "semantics"/>
                  </mrow>
                </mfrac>
              </xsl:if>
            </xsl:for-each>
          </xsl:if>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:apply-templates select = "*[last()]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$GEN_FUN_PREC"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:lowlimit | mml:uplimit | mml:bvar | mml:degree | mml:logbase">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="*" mode = "semantics"/>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:divergence[1] | mml:grad[1] | mml:curl[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo>
        <xsl:if test="*[1]=mml:divergence">
          <xsl:value-of select="'div'"/>
        </xsl:if>
        <xsl:if test="*[1]=mml:grad">
          <xsl:value-of select="'grad'"/>
        </xsl:if>
        <xsl:if test="*[1]=mml:curl">
          <xsl:value-of select="'curl'"/>
        </xsl:if>
      </mo>
      <mspace width="0.01em" linebreak="nobreak"/>
      <xsl:choose>
        <xsl:when test="*[2]=mml:ci">
          <xsl:apply-templates select = "*[2]" mode = "semantics"/>
        </xsl:when>
        <xsl:otherwise>
          <mfenced separators="">
            <xsl:apply-templates select = "*[2]" mode = "semantics"/>
          </mfenced>
        </xsl:otherwise>
      </xsl:choose>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:laplacian[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <msup>
        <mo>
          <xsl:text disable-output-escaping='yes'>&amp;#x2207;</xsl:text>
        </mo>
        <mn> 2 </mn>
      </msup>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2061;</xsl:text>
      </mo>
      <xsl:apply-templates select = "*[2]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$GEN_FUN_PREC"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </mrow>
  </xsl:template>



  <!-- ***************** SET THEORY ***************** -->

  <xsl:template match = "mml:set | mml:list">
    <mfenced>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:if test="self::mml:set">
        <xsl:attribute name="open">
          <xsl:value-of select="'{'"/>
        </xsl:attribute>
        <xsl:attribute name="close">
          <xsl:value-of select="'}'"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:if test="self::mml:list">
        <xsl:attribute name="open">
          <xsl:value-of select="'['"/>
        </xsl:attribute>
        <xsl:attribute name="close">
          <xsl:value-of select="']'"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="not(child::mml:bvar) and not(child::mml:condition)">
          <xsl:apply-templates select = "*" mode="semantics"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:attribute name="separators"/>
          <xsl:apply-templates select = "*[not(self::mml:condition) and not(self::mml:bvar)]" mode="semantics"/>
          <mo lspace="0.1666em" rspace="0.1666em"> | </mo>
          <xsl:apply-templates select="mml:condition" mode = "semantics"/>
        </xsl:otherwise>
      </xsl:choose>
    </mfenced>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:union[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &gt; $UNION_PREC or $IN_PREC=$UNION_PREC
                    and $PARAM=$PAR_SAME">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="union">
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="union">
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="union">
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:union[1]]" mode="union">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:apply-templates select = "*[2]" mode="semantics">
      <xsl:with-param name="IN_PREC" select="$UNION_PREC"/>
      <xsl:with-param name="PARAM" select="$PARAM"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
    </xsl:apply-templates>
    <xsl:for-each select = "*[position()>2]">
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x222A;</xsl:text>
      </mo>
      <xsl:apply-templates select = "." mode="semantics">
        <xsl:with-param name="IN_PREC" select="$UNION_PREC"/>
        <xsl:with-param name="PAREN" select="$PAREN"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </xsl:for-each>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:intersect[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &gt; $INTERSECT_PREC or $IN_PREC=$INTERSECT_PREC
                    and $PARAM=$PAR_SAME">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="intersect">
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="intersect">
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="intersect">
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:apply[mml:intersect[1]]" mode="intersect">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:apply-templates select = "*[2]" mode="semantics">
      <xsl:with-param name="IN_PREC" select="$INTERSECT_PREC"/>
      <xsl:with-param name="PARAM" select="$PARAM"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
    </xsl:apply-templates>
    <xsl:for-each select = "*[position()>2]">
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x2229;</xsl:text>
      </mo>
      <xsl:apply-templates select = "." mode="semantics">
        <xsl:with-param name="IN_PREC" select="$INTERSECT_PREC"/>
        <xsl:with-param name="PAREN" select="$PAREN"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </xsl:for-each>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:setdiff[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &gt; $SETDIFF_PREC or $IN_PREC=$SETDIFF_PREC
                    and $PARAM=$PAR_SAME">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="setdiff">
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="setdiff">
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="setdiff">
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "mml:apply[mml:setdiff[1]]" mode="setdiff">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:apply-templates select = "*[2]" mode = "semantics">
      <xsl:with-param name="IN_PREC" select="$SETDIFF_PREC"/>
      <xsl:with-param name="PARAM" select="$PARAM"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
    </xsl:apply-templates>
    <mo>\</mo>
    <xsl:apply-templates select = "*[3]" mode = "semantics">
      <xsl:with-param name="IN_PREC" select="$SETDIFF_PREC"/>
      <xsl:with-param name="PARAM" select="$PAR_SAME"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
    </xsl:apply-templates>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:cartesianproduct[1]]">
    <xsl:param name="IN_PREC" select="$NO_PREC"/>
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:choose>
      <xsl:when test="$IN_PREC &gt; $CARTPROD_PREC or $IN_PREC=$CARTPROD_PREC
                    and $PARAM=$PAR_SAME">
        <mfenced separators="">
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="cartprod">
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mfenced>
      </xsl:when>
      <xsl:when test="$IN_PREC &gt; $NO_PREC and $IN_PREC &lt; $GEN_FUN_PREC
                    and not($SEM_SW=$SEM_ALL) and not($SEM_SW=$SEM_XREF)
                    and not($SEM_SW=$SEM_XREF_EXT)">
        <xsl:apply-templates select="." mode="cartprod">
          <xsl:with-param name="PARAM" select="$PARAM"/>
          <xsl:with-param name="PAREN" select="$PAREN"/>
          <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
        </xsl:apply-templates>
      </xsl:when>
      <xsl:otherwise>
        <mrow>
          <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
            <xsl:attribute name="xref">
              <xsl:value-of select="@id"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:apply-templates select="." mode="cartprod">
            <xsl:with-param name="PARAM" select="$PARAM"/>
            <xsl:with-param name="PAREN" select="$PAREN"/>
            <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
          </xsl:apply-templates>
        </mrow>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match = "*" mode="cartprod">
    <xsl:param name="PARAM" select="$NO_PARAM"/>
    <xsl:param name="PAREN" select="$PAR_NO"/>
    <xsl:param name="PAR_NO_IGNORE" select="$YES"/>
    <xsl:apply-templates select = "*[2]" mode = "semantics">
      <xsl:with-param name="IN_PREC" select="$CARTPROD_PREC"/>
      <xsl:with-param name="PARAM" select="$PARAM"/>
      <xsl:with-param name="PAREN" select="$PAREN"/>
      <xsl:with-param name="PAR_NO_IGNORE" select="$PAR_NO_IGNORE"/>
    </xsl:apply-templates>
    <xsl:for-each select = "*[position()>2]">
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x00D7;</xsl:text>
      </mo>
      <xsl:apply-templates select = "." mode="semantics">
        <xsl:with-param name="IN_PREC" select="$CARTPROD_PREC"/>
        <xsl:with-param name="PARAM" select="$PAR_SAME"/>
        <xsl:with-param name="PAREN" select="$PAREN"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </xsl:for-each>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:card[1]]">
    <mfenced open="&#x2223;" close="&#x2223;" separators=",">
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:for-each select = "*[position()>1]">
        <xsl:apply-templates select = "." mode="semantics"/>
      </xsl:for-each>
    </mfenced>
  </xsl:template>



  <!-- ***************** SEQUENCES AND SERIES ***************** -->

  <xsl:template match = "mml:apply[mml:sum[1] | mml:product[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="*[2]=mml:bvar and mml:lowlimit and mml:uplimit">
          <munderover>
            <mo>
              <xsl:if test="*[1]=mml:sum">
                <xsl:text disable-output-escaping='yes'>&amp;#x2211;</xsl:text>
              </xsl:if>
              <xsl:if test="*[1]=mml:product">
                <xsl:text disable-output-escaping='yes'>&amp;#x220F;</xsl:text>
              </xsl:if>
            </mo>
            <mrow>
              <xsl:apply-templates select = "*[2]" mode = "semantics"/>
              <mo> = </mo>
              <xsl:apply-templates select = "mml:lowlimit" mode = "semantics"/>
            </mrow>
            <xsl:apply-templates select = "mml:uplimit" mode = "semantics"/>
          </munderover>
          <xsl:apply-templates select = "*[5]" mode = "semantics"/>
        </xsl:when>
        <xsl:when test="*[2]=mml:bvar and *[3]=mml:condition">
          <munder>
            <mo>
              <xsl:if test="*[1]=mml:sum">
                <xsl:text disable-output-escaping='yes'>&amp;#x2211;</xsl:text>
              </xsl:if>
              <xsl:if test="*[1]=mml:product">
                <xsl:text disable-output-escaping='yes'>&amp;#x220F;</xsl:text>
              </xsl:if>
            </mo>
            <xsl:apply-templates select = "*[3]" mode = "semantics"/>
          </munder>
          <xsl:apply-templates select = "*[4]" mode = "semantics"/>
        </xsl:when>
        <xsl:when test="*[2]=mml:domainofapplication">
          <munder>
            <mo>
              <xsl:if test="*[1]=mml:sum">
                <xsl:text disable-output-escaping='yes'>&amp;#x2211;</xsl:text>
              </xsl:if>
              <xsl:if test="*[1]=mml:product">
                <xsl:text disable-output-escaping='yes'>&amp;#x220F;</xsl:text>
              </xsl:if>
            </mo>
            <xsl:apply-templates select="mml:domainofapplication" mode = "semantics"/>
          </munder>
          <mrow>
            <xsl:apply-templates select="*[position()=last()]" mode = "semantics"/>
          </mrow>
        </xsl:when>
      </xsl:choose>
    </mrow>
  </xsl:template>


  <xsl:template match="mml:apply[*[1][self::mml:int]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="mml:domainofapplication">
          <munder>
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x222B;</xsl:text>
            </mo>
            <xsl:apply-templates select="mml:domainofapplication" mode="semantics"/>
          </munder>
        </xsl:when>
        <xsl:when test="mml:condition">
          <munder>
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x222B;</xsl:text>
            </mo>
            <xsl:apply-templates select="mml:condition" mode="semantics"/>
          </munder>
        </xsl:when>
        <xsl:when test="mml:interval">
          <munderover>
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x222B;</xsl:text>
            </mo>
            <mrow>
              <xsl:apply-templates select="mml:interval/*[position()=1]" mode="semantics"/>
            </mrow>
            <mrow>
              <mspace width="1em"/>
              <xsl:apply-templates select="mml:interval/*[position()=2]" mode="semantics"/>
            </mrow>
          </munderover>
        </xsl:when>
        <xsl:when test="mml:lowlimit | mml:uplimit">
          <munderover>
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x222B;</xsl:text>
            </mo>
            <xsl:apply-templates select="mml:lowlimit" mode="semantics"/>
            <mrow>
              <mspace width="1em"/>
              <xsl:apply-templates select="mml:uplimit" mode="semantics"/>
            </mrow>
          </munderover>
        </xsl:when>
        <xsl:otherwise>
          <mo>
            <xsl:text disable-output-escaping='yes'>&amp;#x222B;</xsl:text>
          </mo>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:apply-templates select="*[position()=last() and last()>1 and not(self::mml:domainofapplication) and not(self::mml:condition) and not(self::mml:interval) and not(self::mml:lowlimit) and not(self::mml:uplimit) and not(self::mml:bvar)]" mode="semantics">
        <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
      <xsl:if test="mml:bvar">
        <mrow>
          <mo>
            <xsl:text disable-output-escaping='yes'>&amp;#x2146;</xsl:text>
          </mo>
          <xsl:apply-templates select="mml:bvar" mode="semantics"/>
        </mrow>
      </xsl:if>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:limit[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <munder>
        <mo> lim </mo>
        <mrow>
          <xsl:if test="*[2]=mml:bvar and *[3]=mml:lowlimit">
            <xsl:apply-templates select = "*[2]" mode = "semantics"/>
            <mo>
              <xsl:text disable-output-escaping='yes'>&amp;#x2192;</xsl:text>
            </mo>
            <xsl:apply-templates select = "*[3]" mode = "semantics"/>
          </xsl:if>
          <xsl:if test="*[2]=mml:bvar and *[3]=mml:condition">
            <xsl:apply-templates select = "*[3]" mode = "semantics"/>
          </xsl:if>
        </mrow>
      </munder>
      <xsl:apply-templates select = "*[4]" mode = "semantics"/>
    </mrow>
  </xsl:template>



  <!-- ***************** TRIGONOMETRY ***************** -->

  <xsl:template match = "mml:apply[*[1][self::mml:sin | self::mml:cos |
                       self::mml:tan | self::mml:sec | self::mml:csc |
                       self::mml:cot | self::mml:sinh | self::mml:cosh |
                       self::mml:tanh | self::mml:sech | self::mml:csch |
                       self::mml:coth | self::mml:arcsin | self::mml:arccos |
                       self::mml:arctan | self::mml:arcsec | self::mml:arccsc |
                       self::mml:arccot | self::mml:arcsinh | self::mml:arccosh |
                       self::mml:arctanh | self::mml:arcsech | self::mml:arccsch |
                       self::mml:arccoth]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:if test="not(parent::mml:apply[mml:power[1]])">
        <xsl:apply-templates select = "*[1]" mode = "trigonometry"/>
      </xsl:if>
      <xsl:if test="parent::mml:apply[mml:power[1]]">
        <msup>
          <xsl:apply-templates select = "*[1]" mode = "trigonometry"/>
          <xsl:apply-templates select = "../*[3]" mode = "semantics"/>
        </msup>
      </xsl:if>
      <mspace width="0.01em" linebreak="nobreak"/>
      <xsl:apply-templates select = "*[2]" mode = "semantics">
        <xsl:with-param name="IN_PREC" select="$FUNCTN_PREC"/>
        <xsl:with-param name="PAR_NO_IGNORE" select="$NO"/>
      </xsl:apply-templates>
    </mrow>
  </xsl:template>

  <xsl:template match = "mml:sin | mml:cos |
                       mml:tan | mml:sec | mml:csc |
                       mml:cot | mml:sinh | mml:cosh |
                       mml:tanh | mml:sech | mml:csch |
                       mml:coth | mml:arcsin | mml:arccos |
                       mml:arctan | mml:arcsec | mml:arccsc |
                       mml:arccot | mml:arcsinh | mml:arccosh |
                       mml:arctanh | mml:arcsech | mml:arccsch |
                       mml:arccoth">
    <xsl:apply-templates select = "." mode = "trigonometry"/>
  </xsl:template>

  <xsl:template match = "*" mode="trigonometry">
    <mo>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="self::mml:sin">
          <xsl:value-of select="'sin'"/>
        </xsl:when>
        <xsl:when test="self::mml:cos">
          <xsl:value-of select="'cos'"/>
        </xsl:when>
        <xsl:when test="self::mml:tan">
          <xsl:value-of select="'tan'"/>
        </xsl:when>
        <xsl:when test="self::mml:sec">
          <xsl:value-of select="'sec'"/>
        </xsl:when>
        <xsl:when test="self::mml:csc">
          <xsl:value-of select="'csc'"/>
        </xsl:when>
        <xsl:when test="self::mml:cot">
          <xsl:value-of select="'cot'"/>
        </xsl:when>
        <xsl:when test="self::mml:sinh">
          <xsl:value-of select="'sinh'"/>
        </xsl:when>
        <xsl:when test="self::mml:cosh">
          <xsl:value-of select="'cosh'"/>
        </xsl:when>
        <xsl:when test="self::mml:tanh">
          <xsl:value-of select="'tanh'"/>
        </xsl:when>
        <xsl:when test="self::mml:sech">
          <xsl:value-of select="'sech'"/>
        </xsl:when>
        <xsl:when test="self::mml:csch">
          <xsl:value-of select="'csch'"/>
        </xsl:when>
        <xsl:when test="self::mml:coth">
          <xsl:value-of select="'coth'"/>
        </xsl:when>
        <xsl:when test="self::mml:arcsin">
          <xsl:value-of select="'arcsin'"/>
        </xsl:when>
        <xsl:when test="self::mml:arccos">
          <xsl:value-of select="'arccos'"/>
        </xsl:when>
        <xsl:when test="self::mml:arctan">
          <xsl:value-of select="'arctan'"/>
        </xsl:when>
        <xsl:when test="self::mml:arcsec">
          <xsl:value-of select="'arcsec'"/>
        </xsl:when>
        <xsl:when test="self::mml:arccsc">
          <xsl:value-of select="'arccsc'"/>
        </xsl:when>
        <xsl:when test="self::mml:arccot">
          <xsl:value-of select="'arccot'"/>
        </xsl:when>
        <xsl:when test="self::mml:arcsinh">
          <xsl:value-of select="'arcsinh'"/>
        </xsl:when>
        <xsl:when test="self::mml:arccosh">
          <xsl:value-of select="'arccosh'"/>
        </xsl:when>
        <xsl:when test="self::mml:arctanh">
          <xsl:value-of select="'arctanh'"/>
        </xsl:when>
        <xsl:when test="self::mml:arcsech">
          <xsl:value-of select="'arcsech'"/>
        </xsl:when>
        <xsl:when test="self::mml:arccsch">
          <xsl:value-of select="'arccsch'"/>
        </xsl:when>
        <xsl:when test="self::mml:arccoth">
          <xsl:value-of select="'arccot'"/>
        </xsl:when>
      </xsl:choose>
    </mo>
  </xsl:template>



  <!-- ***************** STATISTICS ***************** -->

  <xsl:template match = "mml:apply[mml:mean[1]]">
    <mfenced open="&#x2329;" close="&#x232A;" separators=",">
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:for-each select = "*[position()>1]">
        <xsl:apply-templates select = "." mode="semantics"/>
      </xsl:for-each>
    </mfenced>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:sdev[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x03C3;</xsl:text>
      </mo>
      <mfenced separators=",">
        <xsl:for-each select = "*[position()>1]">
          <xsl:apply-templates select = "." mode="semantics"/>
        </xsl:for-each>
      </mfenced>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:variance[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo>
        <xsl:text disable-output-escaping='yes'>&amp;#x03C3;</xsl:text>
      </mo>
      <msup>
        <mfenced separators=",">
          <xsl:for-each select = "*[position()>1]">
            <xsl:apply-templates select = "." mode="semantics"/>
          </xsl:for-each>
        </mfenced>
        <mn> 2 </mn>
      </msup>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:median[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo> median </mo>
      <mfenced separators=",">
        <xsl:for-each select = "*[position()>1]">
          <xsl:apply-templates select = "." mode="semantics"/>
        </xsl:for-each>
      </mfenced>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:mode[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo> mode </mo>
      <mfenced separators=",">
        <xsl:for-each select = "*[position()>1]">
          <xsl:apply-templates select = "." mode="semantics"/>
        </xsl:for-each>
      </mfenced>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:moment[1]]">
    <mfenced open="&#x2329;" close="&#x232A;" separators="">
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:if test="*[2]=mml:degree and not(*[3]=mml:momentabout)">
        <msup>
          <xsl:apply-templates select="*[3]" mode = "semantics"/>
          <xsl:apply-templates select="*[2]" mode = "semantics"/>
        </msup>
      </xsl:if>
      <xsl:if test="*[2]=mml:degree and *[3]=mml:momentabout">
        <msup>
          <xsl:apply-templates select="*[4]" mode = "semantics"/>
          <xsl:apply-templates select="*[2]" mode = "semantics"/>
        </msup>
      </xsl:if>
      <xsl:if test="not(*[2]=mml:degree) and *[2]=mml:momentabout">
        <xsl:for-each select = "*[position()>2]">
          <xsl:apply-templates select = "." mode="semantics"/>
        </xsl:for-each>
      </xsl:if>
      <xsl:if test="not(*[2]=mml:degree) and not(*[2]=mml:momentabout)">
        <xsl:for-each select = "*[position()>1]">
          <xsl:apply-templates select = "." mode="semantics"/>
        </xsl:for-each>
      </xsl:if>
    </mfenced>
  </xsl:template>



  <!-- ***************** LINEAR ALGEBRA ***************** -->

  <xsl:template match="mml:vector">
    <mfenced separators="">
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mtable columnalign="center">
        <xsl:for-each select="*">
          <mtr>
            <mtd>
              <xsl:apply-templates select="." mode = "semantics"/>
            </mtd>
          </mtr>
        </xsl:for-each>
      </mtable>
    </mfenced>
  </xsl:template>


  <xsl:template match = "mml:matrix">
    <mfenced separators="">
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mtable>
        <xsl:apply-templates mode = "semantics"/>
      </mtable>
    </mfenced>
  </xsl:template>


  <xsl:template match = "mml:matrixrow">
    <mtr>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:for-each select="*">
        <mtd>
          <xsl:apply-templates select="." mode = "semantics"/>
        </mtd>
      </xsl:for-each>
    </mtr>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:determinant[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <mo> det </mo>
      <mspace width="0.2em" linebreak="nobreak"/>
      <xsl:apply-templates select = "*[2]" mode = "semantics"/>
    </mrow>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:transpose[1]]">
    <msup>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select = "*[2]" mode = "semantics"/>
      <mo> T </mo>
    </msup>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:selector[1]]">
    <msub>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="*[2]" mode = "semantics"/>
      <mfenced open="" close="">
        <xsl:for-each select="*[position()>2]">
          <xsl:apply-templates select="." mode = "semantics"/>
        </xsl:for-each>
      </mfenced>
    </msub>
  </xsl:template>


  <xsl:template match = "mml:apply[mml:vectorproduct[1] |
                                 mml:scalarproduct[1] | mml:outerproduct[1]]">
    <mrow>
      <xsl:if test="($SEM_SW=$SEM_XREF or $SEM_SW=$SEM_XREF_EXT) and @id">
        <xsl:attribute name="xref">
          <xsl:value-of select="@id"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="*[2]" mode = "semantics"/>
      <mo>
        <xsl:if test="mml:vectorproduct[1]">
          <xsl:text disable-output-escaping='yes'>&amp;#x00D7;</xsl:text>
        </xsl:if>
        <xsl:if test="mml:scalarproduct[1]">
          <xsl:text disable-output-escaping='yes'>&amp;#x22C5;</xsl:text>
        </xsl:if>
        <xsl:if test="mml:outerproduct[1]">
          <xsl:text disable-output-escaping='yes'>&amp;#x2297;</xsl:text>
        </xsl:if>
      </mo>
      <xsl:apply-templates select="*[3]" mode = "semantics"/>
    </mrow>
  </xsl:template>



  <!-- ***************** CONSTANT and SYMBOL ELEMENTS ***************** -->

  <xsl:template match="mml:integers">
    <mo>
      <xsl:text disable-output-escaping='yes'>&amp;#x2124;</xsl:text>
    </mo>
  </xsl:template>

  <xsl:template match="mml:reals">
    <mo>
      <xsl:text disable-output-escaping='yes'>&amp;#x211D;</xsl:text>
    </mo>
  </xsl:template>

  <xsl:template match="mml:rationals">
    <mo>
      <xsl:text disable-output-escaping='yes'>&amp;#x211A;</xsl:text>
    </mo>
  </xsl:template>

  <xsl:template match="mml:naturalnumbers">
    <mo>
      <xsl:text disable-output-escaping='yes'>&amp;#x2115;</xsl:text>
    </mo>
  </xsl:template>

  <xsl:template match="mml:complexes">
    <mo>
      <xsl:text disable-output-escaping='yes'>&amp;#x2102;</xsl:text>
    </mo>
  </xsl:template>

  <xsl:template match="mml:primes">
    <mo>
      <xsl:text disable-output-escaping='yes'>&amp;#x2119;</xsl:text>
    </mo>
  </xsl:template>

  <xsl:template match="mml:exponentiale">
    <mn>
      <xsl:text disable-output-escaping='yes'>&amp;#x2147;</xsl:text>
    </mn>
  </xsl:template>

  <xsl:template match="mml:imaginaryi">
    <mn>
      <xsl:text disable-output-escaping='yes'>&amp;#x2148;</xsl:text>
    </mn>
  </xsl:template>

  <xsl:template match="mml:notanumber">
    <mo> NaN </mo>
  </xsl:template>

  <xsl:template match="mml:true">
    <mo> true </mo>
  </xsl:template>

  <xsl:template match="mml:false">
    <mo> false </mo>
  </xsl:template>

  <xsl:template match="mml:emptyset">
    <mo>
      <xsl:text disable-output-escaping='yes'>&amp;#x2205;</xsl:text>
    </mo>
  </xsl:template>

  <xsl:template match="mml:pi">
    <mn>
      <xsl:text disable-output-escaping='yes'>&amp;#x03C0;</xsl:text>
    </mn>
  </xsl:template>

  <xsl:template match="mml:eulergamma">
    <mo>
      <xsl:text disable-output-escaping='yes'>&amp;#x213D;</xsl:text>
    </mo>
  </xsl:template>

  <xsl:template match="mml:infinity">
    <mo>
      <xsl:text disable-output-escaping='yes'>&amp;#x221E;</xsl:text>
    </mo>
  </xsl:template>

</xsl:stylesheet>
