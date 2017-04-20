<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="xml" />
  <xsl:template match="math">
    <math>
      <xsl:apply-templates/>
    </math>
  </xsl:template>

  <!-- 4.4.1.1 cn -->

  <xsl:template match="cn">
    <mn>
      <xsl:apply-templates/>
    </mn>
  </xsl:template>

  <xsl:template match="cn[@type='complex-cartesian']">
    <mrow>
      <mn>
        <xsl:apply-templates select="text()[1]"/>
      </mn>
      <mo>+</mo>
      <mn>
        <xsl:apply-templates select="text()[2]"/>
      </mn>
      <mo>
        <!--&#8290;-->
        <!--invisible times-->
      </mo>
      <mi>
        i<!-- imaginary i -->
      </mi>
    </mrow>
  </xsl:template>

  <xsl:template match="cn[@type='rational']">
    <mrow>
      <mn>
        <xsl:apply-templates select="text()[1]"/>
      </mn>
      <mo>/</mo>
      <mn>
        <xsl:apply-templates select="text()[2]"/>
      </mn>
    </mrow>
  </xsl:template>

  <xsl:template match="cn[@type='integer']">
    <xsl:choose>
      <xsl:when test="not(@base) or @base=10">
        <mn>
          <xsl:apply-templates/>
        </mn>
      </xsl:when>
      <xsl:otherwise>
        <msub>
          <mn>
            <xsl:apply-templates/>
          </mn>
          <mn>
            <xsl:value-of select="@base"/>
          </mn>
        </msub>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="cn[@type='complex-polar']">
    <mrow>
      <mn>
        <xsl:apply-templates select="text()[1]"/>
      </mn>
      <mo>
        <!--&#8290;-->
        <!--invisible times-->
      </mo>
      <msup>
        <mi>
          e<!-- exponential e-->
        </mi>
        <mrow>
          <mi>
            i<!-- imaginary i-->
          </mi>
          <mo>
            <!--&#8290;-->
            <!--invisible times-->
          </mo>
          <mn>
            <xsl:apply-templates select="text()[2]"/>
          </mn>
        </mrow>
      </msup>
    </mrow>
  </xsl:template>

  <xsl:template match="cn[@type='e-notation']">
    <mn>
      <xsl:apply-templates select="text()[1]"/>E<xsl:apply-templates select="text()[2]"/>
    </mn>
  </xsl:template>

  <!-- 4.4.1.1 ci  -->

  <xsl:template match="ci/text()">
    <mi>
      <xsl:value-of select="."/>
    </mi>
  </xsl:template>

  <xsl:template match="ci">
    <mrow>
      <xsl:apply-templates/>
    </mrow>
  </xsl:template>

  <!-- 4.4.1.2 csymbol -->

  <xsl:template match="csymbol/text()">
    <mo>
      <xsl:apply-templates/>
    </mo>
  </xsl:template>

  <xsl:template match="csymbol">
    <mrow>
      <xsl:apply-templates/>
    </mrow>
  </xsl:template>

  <!-- 4.4.2.1 apply 4.4.2.2 reln -->

  <xsl:template match="apply|reln">
    <mrow>
      <xsl:apply-templates select="*[1]">
        <xsl:with-param name="p" select="10"/>
      </xsl:apply-templates>
      <mo>
        <!--&#8290;-->
        <!--invisible times-->
      </mo>
      <mfenced open="(" close=")" separators=",">
        <xsl:apply-templates select="*[position()>1]"/>
      </mfenced>
    </mrow>
  </xsl:template>

  <!-- 4.4.2.3 fn -->
  <xsl:template match="fn">
    <mrow>
      <xsl:apply-templates/>
    </mrow>
  </xsl:template>

  <!-- 4.4.2.4 interval -->
  <xsl:template match="interval[*[2]]">
    <mfenced open="[" close="]">
      <xsl:apply-templates/>
    </mfenced>
  </xsl:template>
  <xsl:template match="interval[*[2]][@closure='open']">
    <mfenced open="(" close=")">
      <xsl:apply-templates/>
    </mfenced>
  </xsl:template>
  <xsl:template match="interval[*[2]][@closure='open-closed']">
    <mfenced open="(" close="]">
      <xsl:apply-templates/>
    </mfenced>
  </xsl:template>
  <xsl:template match="interval[*[2]][@closure='closed-open']">
    <mfenced open="[" close=")">
      <xsl:apply-templates/>
    </mfenced>
  </xsl:template>

  <xsl:template match="interval">
    <mfenced open="{{" close="}}">
      <xsl:apply-templates/>
    </mfenced>
  </xsl:template>

  <!-- 4.4.2.5 inverse -->

  <xsl:template match="apply[*[1][self::inverse]]">
    <msup>
      <xsl:apply-templates select="*[2]"/>
      <mrow>
        <mo>(</mo>
        <mn>-1</mn>
        <mo>)</mo>
      </mrow>
    </msup>
  </xsl:template>

  <!-- 4.4.2.6 sep -->

  <!-- 4.4.2.7 condition -->
  <xsl:template match="condition">
    <mrow>
      <xsl:apply-templates/>
    </mrow>
  </xsl:template>

  <!-- 4.4.2.8 declare -->
  <xsl:template match="declare"/>

  <!-- 4.4.2.9 lambda -->
  <xsl:template match="apply[*[1][self::lambda]]">
    <mrow>
      <mi>
        &#955;<!--lambda-->
      </mi>
      <mrow>
        <xsl:apply-templates select="bvar/*"/>
      </mrow>
      <mo>.</mo>
      <mfenced>
        <xsl:apply-templates select="*[last()]"/>
      </mfenced>
    </mrow>
  </xsl:template>


  <!-- 4.4.2.10 compose -->
  <xsl:template match="apply[*[1][self::compose]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="1"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8728;<!-- o -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>


  <!-- 4.4.2.11` ident -->
  <xsl:template match="ident">
    <mo>id</mo>
  </xsl:template>

  <!-- 4.4.2.12` domain -->
  <xsl:template match="domain">
    <mo>domain</mo>
  </xsl:template>

  <!-- 4.4.2.13` codomain -->
  <xsl:template match="codomain">
    <mo>codomain</mo>
  </xsl:template>

  <!-- 4.4.2.14` image -->
  <xsl:template match="image">
    <mo>image</mo>
  </xsl:template>

  <!-- 4.4.2.15` domainofapplication -->
  <xsl:template match="domainofapplication">
    <error/>
  </xsl:template>

  <!-- 4.4.2.16` piecewise -->
  <xsl:template match="piecewise">
    <mrow>
      <mo>{</mo>
      <mtable>
        <xsl:for-each select="piece|otherwise">
          <mtr>
            <mtd>
              <xsl:apply-templates select="*[1]"/>
            </mtd>
            <mtd>
              <mtext>&#160; if &#160;</mtext>
            </mtd>
            <mtd>
              <xsl:apply-templates select="*[2]"/>
            </mtd>
          </mtr>
        </xsl:for-each>
      </mtable>
    </mrow>
  </xsl:template>


  <!-- 4.4.3.1 quotient -->
  <xsl:template match="apply[*[1][self::quotient]]">
    <mrow>
      <mo>
        &#8970;<!-- lfloor-->
      </mo>
      <xsl:apply-templates select="*[2]"/>
      <mo>/</mo>
      <xsl:apply-templates select="*[3]"/>
      <mo>
        &#8971;<!-- rfloor-->
      </mo>
    </mrow>
  </xsl:template>



  <!-- 4.4.3.2 factorial -->
  <xsl:template match="apply[*[1][self::factorial]]">
    <mrow>
      <xsl:apply-templates select="*[2]">
        <xsl:with-param name="p" select="7"/>
      </xsl:apply-templates>
      <mo>!</mo>
    </mrow>
  </xsl:template>


  <!-- 4.4.3.3 divide -->
  <xsl:template match="apply[*[1][self::divide]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="mo">
        <mo>/</mo>
      </xsl:with-param>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="this-p" select="3"/>
    </xsl:call-template>
  </xsl:template>


  <!-- 4.4.3.4 max  min-->
  <xsl:template match="apply[*[1][self::max]]">
    <mrow>
      <mo>max</mo>
      <xsl:call-template name="set"/>
    </mrow>
  </xsl:template>

  <xsl:template match="apply[*[1][self::min]]">
    <mrow>
      <mo>max</mo>
      <xsl:call-template name="set"/>
    </mrow>
  </xsl:template>

  <!-- 4.4.3.5  minus-->
  <xsl:template match="apply[*[1][self::minus] and count(*)=2]">
    <mrow>
      <mo>
        &#8722;<!--minus-->
      </mo>
      <xsl:apply-templates select="*[2]">
        <xsl:with-param name="p" select="5"/>
      </xsl:apply-templates>
    </mrow>
  </xsl:template>

  <xsl:template match="apply[*[1][self::minus] and count(*)&gt;2]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="mo">
        <mo>
          &#8722;<!--minus-->
        </mo>
      </xsl:with-param>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="this-p" select="2"/>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.3.6  plus-->
  <xsl:template match="apply[*[1][self::plus]]">
    <xsl:param name="p" select="0"/>
    <mrow>
      <xsl:if test="$p &gt; 2">
        <mo>(</mo>
      </xsl:if>
      <xsl:for-each select="*[position()&gt;1]">
        <xsl:if test="position() &gt; 1">
          <mo>
            <xsl:choose>
              <xsl:when test="self::apply[*[1][self::times] and
      *[2][self::apply/*[1][self::minus] or self::cn[not(sep) and
      (number(.) &lt; 0)]]]">
                &#8722;<!--minus-->
              </xsl:when>
              <xsl:otherwise>+</xsl:otherwise>
            </xsl:choose>
          </mo>
        </xsl:if>
        <xsl:choose>
          <xsl:when test="self::apply[*[1][self::times] and
      *[2][self::cn[not(sep) and (number(.) &lt;0)]]]">
            <mrow>
              <mn>
                <xsl:value-of select="-(*[2])"/>
              </mn>
              <mo>
                <!--&#8290;-->
                <!--invisible times-->
              </mo>
              <xsl:apply-templates select=".">
                <xsl:with-param name="first" select="2"/>
                <xsl:with-param name="p" select="2"/>
              </xsl:apply-templates>
            </mrow>
          </xsl:when>
          <xsl:when test="self::apply[*[1][self::times] and
      *[2][self::apply/*[1][self::minus]]]">
            <mrow>
              <xsl:apply-templates select="./*[2]/*[2]"/>
              <xsl:apply-templates select=".">
                <xsl:with-param name="first" select="2"/>
                <xsl:with-param name="p" select="2"/>
              </xsl:apply-templates>
            </mrow>
          </xsl:when>
          <xsl:otherwise>
            <xsl:apply-templates select=".">
              <xsl:with-param name="p" select="2"/>
            </xsl:apply-templates>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
      <xsl:if test="$p &gt; 2">
        <mo>)</mo>
      </xsl:if>
    </mrow>
  </xsl:template>


  <!-- 4.4.3.7 power -->
  <xsl:template match="apply[*[1][self::power]]">
    <msup>
      <xsl:apply-templates select="*[2]">
        <xsl:with-param name="p" select="5"/>
      </xsl:apply-templates>
      <xsl:apply-templates select="*[3]">
        <xsl:with-param name="p" select="5"/>
      </xsl:apply-templates>
    </msup>
  </xsl:template>

  <!-- 4.4.3.8 remainder -->
  <xsl:template match="apply[*[1][self::rem]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="mo">
        <mo>mod</mo>
      </xsl:with-param>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="this-p" select="3"/>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.3.9  times-->
  <xsl:template match="apply[*[1][self::times]]" name="times">
    <xsl:param name="p" select="0"/>
    <xsl:param name="first" select="1"/>
    <mrow>
      <xsl:if test="$p &gt; 3">
        <mo>(</mo>
      </xsl:if>
      <xsl:for-each select="*[position()&gt;1]">
        <xsl:if test="position() &gt; 1">
          <mo>
            <xsl:choose>
              <xsl:when test="self::cn">
                &#215;<!-- times -->
              </xsl:when>
              <xsl:otherwise>
                <!--&#8290;-->
                <!--invisible times-->
              </xsl:otherwise>
            </xsl:choose>
          </mo>
        </xsl:if>
        <xsl:if test="position()&gt;= $first">
          <xsl:apply-templates select=".">
            <xsl:with-param name="p" select="3"/>
          </xsl:apply-templates>
        </xsl:if>
      </xsl:for-each>
      <xsl:if test="$p &gt; 3">
        <mo>)</mo>
      </xsl:if>
    </mrow>
  </xsl:template>


  <!-- 4.4.3.10 root -->
  <xsl:template match="apply[*[1][self::root] and not(degree) or degree=2]" priority="4">
    <msqrt>
      <xsl:apply-templates select="*[position()&gt;1]"/>
    </msqrt>
  </xsl:template>

  <xsl:template match="apply[*[1][self::root]]">
    <mroot>
      <xsl:apply-templates select="*[position()&gt;1 and not(self::degree)]"/>
      <mrow>
        <xsl:apply-templates select="degree/*"/>
      </mrow>
    </mroot>
  </xsl:template>

  <!-- 4.4.3.11 gcd -->
  <xsl:template match="gcd">
    <mo>gcd</mo>
  </xsl:template>

  <!-- 4.4.3.12 and -->
  <xsl:template match="apply[*[1][self::and]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="2"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8743;<!-- and -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>


  <!-- 4.4.3.13 or -->
  <xsl:template match="apply[*[1][self::or]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="3"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8744;<!-- or -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.3.14 xor -->
  <xsl:template match="apply[*[1][self::xor]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="3"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>xor</mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>


  <!-- 4.4.3.15 not -->
  <xsl:template match="apply[*[1][self::not]]">
    <mrow>
      <mo>
        &#172;<!-- not -->
      </mo>
      <xsl:apply-templates select="*[2]">
        <xsl:with-param name="p" select="7"/>
      </xsl:apply-templates>
    </mrow>
  </xsl:template>




  <!-- 4.4.3.16 implies -->
  <xsl:template match="apply[*[1][self::implies]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="mo">
        <mo>
          &#8658;<!-- Rightarrow -->
        </mo>
      </xsl:with-param>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="this-p" select="3"/>
    </xsl:call-template>
  </xsl:template>


  <!-- 4.4.3.17 forall -->
  <xsl:template match="apply[*[1][self::forall]]">
    <mrow>
      <mi>
        &#8704;<!--forall-->
      </mi>
      <mrow>
        <xsl:apply-templates select="bvar[not(current()/condition)]/*|condition/*"/>
      </mrow>
      <mo>.</mo>
      <mfenced>
        <xsl:apply-templates select="*[last()]"/>
      </mfenced>
    </mrow>
  </xsl:template>



  <!-- 4.4.3.18 exists -->
  <xsl:template match="apply[*[1][self::exists]]">
    <mrow>
      <mi>
        &#8707;<!--exists-->
      </mi>
      <mrow>
        <xsl:apply-templates select="bvar[not(current()/condition)]/*|condition/*"/>
      </mrow>
      <mo>.</mo>
      <mfenced>
        <xsl:apply-templates select="*[last()]"/>
      </mfenced>
    </mrow>
  </xsl:template>


  <!-- 4.4.3.19 abs -->
  <xsl:template match="apply[*[1][self::abs]]">
    <mrow>
      <mo>|</mo>
      <xsl:apply-templates select="*[2]"/>
      <mo>|</mo>
    </mrow>
  </xsl:template>



  <!-- 4.4.3.20 conjugate -->
  <xsl:template match="apply[*[1][self::conjugate]]">
    <mover>
      <xsl:apply-templates select="*[2]"/>
      <mo>
        &#175;<!-- overline -->
      </mo>
    </mover>
  </xsl:template>

  <!-- 4.4.3.21 arg -->
  <xsl:template match="arg">
    <mo>arg</mo>
  </xsl:template>


  <!-- 4.4.3.22 real -->
  <xsl:template match="real">
    <mo>
      &#8475;<!-- real -->
    </mo>
  </xsl:template>

  <!-- 4.4.3.23 imaginary -->
  <xsl:template match="imaginary">
    <mo>
      &#8465;<!-- imaginary -->
    </mo>
  </xsl:template>

  <!-- 4.4.3.24 lcm -->
  <xsl:template match="lcm">
    <mo>lcm</mo>
  </xsl:template>


  <!-- 4.4.3.25 floor -->
  <xsl:template match="apply[*[1][self::floor]]">
    <mrow>
      <mo>
        &#8970;<!-- lfloor-->
      </mo>
      <xsl:apply-templates select="*[2]"/>
      <mo>
        &#8971;<!-- rfloor-->
      </mo>
    </mrow>
  </xsl:template>


  <!-- 4.4.3.25 ceiling -->
  <xsl:template match="apply[*[1][self::ceiling]]">
    <mrow>
      <mo>
        &#8968;<!-- lceil-->
      </mo>
      <xsl:apply-templates select="*[2]"/>
      <mo>
        &#8969;<!-- rceil-->
      </mo>
    </mrow>
  </xsl:template>

  <!-- 4.4.4.1 eq -->
  <xsl:template match="apply[*[1][self::eq]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="1"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>=</mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.4.2 neq -->
  <xsl:template match="apply[*[1][self::neq]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="1"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8800;<!-- neq -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.4.3 eq -->
  <xsl:template match="apply[*[1][self::gt]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="1"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>&gt;</mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.4.4 lt -->
  <xsl:template match="apply[*[1][self::lt]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="1"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>&lt;</mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.4.5 geq -->
  <xsl:template match="apply[*[1][self::geq]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="1"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>&#8805;</mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.4.6 geq -->
  <xsl:template match="apply[*[1][self::leq]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="1"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>&#8804;</mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.4.7 equivalent -->
  <xsl:template match="apply[*[1][self::equivalent]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="1"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>&#8801;</mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.4.8 approx -->
  <xsl:template match="apply[*[1][self::approx]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="1"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>&#8771;</mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>


  <!-- 4.4.4.9 factorof -->
  <xsl:template match="apply[*[1][self::factorof]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="mo">
        <mo>|</mo>
      </xsl:with-param>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="this-p" select="3"/>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.5.1 int -->
  <xsl:template match="apply[*[1][self::int]]">
    <mrow>
      <msubsup>
        <mi>
          &#8747;<!--int-->
        </mi>
        <mrow>
          <xsl:apply-templates select="lowlimit/*|interval/*[1]|condition/*"/>
        </mrow>
        <mrow>
          <xsl:apply-templates select="uplimit/*|interval/*[2]"/>
        </mrow>
      </msubsup>
      <xsl:apply-templates select="*[last()]"/>
      <mo>d</mo>
      <xsl:apply-templates select="bvar"/>
    </mrow>
  </xsl:template>

  <!-- 4.4.5.2 diff -->
  <xsl:template match="apply[*[1][self::diff] and ci and count(*)=2]" priority="2">
    <msup>
      <mrow>
        <xsl:apply-templates select="*[2]"/>
      </mrow>
      <mo>
        &#8242;<!--prime-->
      </mo>
    </msup>
  </xsl:template>

  <xsl:template match="apply[*[1][self::diff]]" priority="1">
    <mfrac>
      <xsl:choose>
        <xsl:when test="bvar/degree">
          <mrow>
            <msup>
              <mo>d</mo>
              <xsl:apply-templates select="bvar/degree/node()"/>
            </msup>
            <xsl:apply-templates  select="*[last()]"/>
          </mrow>
          <mrow>
            <mo>d</mo>
            <msup>
              <xsl:apply-templates
 select="bvar/node()"/>
              <xsl:apply-templates
 select="bvar/degree/node()"/>
            </msup>
          </mrow>
        </xsl:when>
        <xsl:otherwise>
          <mrow>
            <mo>d</mo>
            <xsl:apply-templates select="*[last()]"/>
          </mrow>
          <mrow>
            <mo>d</mo>
            <xsl:apply-templates select="bvar"/>
          </mrow>
        </xsl:otherwise>
      </xsl:choose>
    </mfrac>
  </xsl:template>


  <!-- 4.4.5.3 partialdiff -->
  <xsl:template match="apply[*[1][self::partialdiff] and list and ci and count(*)=3]" priority="2">
    <mrow>
      <msub>
        <mo>D</mo>
        <mrow>
          <xsl:for-each select="list[1]/*">
            <xsl:apply-templates select="."/>
            <xsl:if test="position()&lt;last()">
              <mo>,</mo>
            </xsl:if>
          </xsl:for-each>
        </mrow>
      </msub>
      <mrow>
        <xsl:apply-templates select="*[3]"/>
      </mrow>
    </mrow>
  </xsl:template>

  <xsl:template match="apply[*[1][self::partialdiff]]" priority="1">
    <mfrac>
      <mrow>
        <msup>
          <mo>
            &#8706;<!-- partial -->
          </mo>
          <mrow>
            <xsl:choose>
              <xsl:when test="degree">
                <xsl:apply-templates select="degree/node()"/>
              </xsl:when>
              <xsl:when test="bvar/degree[string(number(.))='NaN']">
                <xsl:for-each select="bvar/degree">
                  <xsl:apply-templates select="node()"/>
                  <xsl:if test="position()&lt;last()">
                    <mo>+</mo>
                  </xsl:if>
                </xsl:for-each>
                <xsl:if test="count(bvar[not(degree)])&gt;0">
                  <mo>+</mo>
                  <mn>
                    <xsl:value-of select="count(bvar[not(degree)])"/>
                  </mn>
                </xsl:if>
              </xsl:when>
              <xsl:otherwise>
                <mn>
                  <xsl:value-of select="sum(bvar/degree)+count(bvar[not(degree)])"/>
                </mn>
              </xsl:otherwise>
            </xsl:choose>
          </mrow>
        </msup>
        <xsl:apply-templates  select="*[last()]"/>
      </mrow>
      <mrow>
        <xsl:for-each select="bvar">
          <mrow>
            <mo>
              &#8706;<!-- partial -->
            </mo>
            <msup>
              <xsl:apply-templates select="node()"/>
              <mrow>
                <xsl:apply-templates select="degree/node()"/>
              </mrow>
            </msup>
          </mrow>
        </xsl:for-each>
      </mrow>
    </mfrac>
  </xsl:template>

  <!-- 4.4.5.4  lowlimit-->
  <xsl:template match="lowlimit"/>

  <!-- 4.4.5.5 uplimit-->
  <xsl:template match="uplimit"/>

  <!-- 4.4.5.6  bvar-->
  <xsl:template match="bvar">
    <mi>
      <xsl:apply-templates/>
    </mi>
    <xsl:if test="following-sibling::bvar">
      <mo>,</mo>
    </xsl:if>
  </xsl:template>

  <!-- 4.4.5.7 degree-->
  <xsl:template match="degree"/>

  <!-- 4.4.5.8 divergence-->
  <xsl:template match="divergence">
    <mo>div</mo>
  </xsl:template>

  <!-- 4.4.5.9 grad-->
  <xsl:template match="grad">
    <mo>grad</mo>
  </xsl:template>

  <!-- 4.4.5.10 curl -->
  <xsl:template match="curl">
    <mo>curl</mo>
  </xsl:template>


  <!-- 4.4.5.11 laplacian-->
  <xsl:template match="laplacian">
    <msup>
      <mo>
        &#8711;<!-- nabla -->
      </mo>
      <mn>2</mn>
    </msup>
  </xsl:template>

  <!-- 4.4.6.1 set -->

  <xsl:template match="set">
    <xsl:call-template name="set"/>
  </xsl:template>

  <!-- 4.4.6.2 list -->

  <xsl:template match="list">
    <xsl:call-template name="set">
      <xsl:with-param name="o" select="'('"/>
      <xsl:with-param name="c" select="')'"/>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.6.3 union -->
  <xsl:template match="apply[*[1][self::union]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="2"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8746;<!-- union -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.6.4 intersect -->
  <xsl:template match="apply[*[1][self::intersect]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="3"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8745;<!-- intersect -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.6.5 in -->
  <xsl:template match="apply[*[1][self::in]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="mo">
        <mo>
          &#8712;<!-- in -->
        </mo>
      </xsl:with-param>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="this-p" select="3"/>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.6.5 notin -->
  <xsl:template match="apply[*[1][self::notin]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="mo">
        <mo>
          &#8713;<!-- not in -->
        </mo>
      </xsl:with-param>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="this-p" select="3"/>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.6.7 subset -->
  <xsl:template match="apply[*[1][self::subset]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="2"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8838;<!-- subseteq -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.6.8 prsubset -->
  <xsl:template match="apply[*[1][self::prsubset]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="2"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8834;<!-- prsubset -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.6.9 notsubset -->
  <xsl:template match="apply[*[1][self::notsubset]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="this-p" select="2"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8840;<!-- notsubseteq -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.6.10 notprsubset -->
  <xsl:template match="apply[*[1][self::notprsubset]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="this-p" select="2"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8836;<!-- prsubset -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.6.11 setdiff -->
  <xsl:template match="apply[*[1][self::setdiff]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="this-p" select="2"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#8726;<!-- setminus -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.6.12 card -->
  <xsl:template match="apply[*[1][self::card]]">
    <mrow>
      <mo>|</mo>
      <xsl:apply-templates select="*[2]"/>
      <mo>|</mo>
    </mrow>
  </xsl:template>

  <!-- 4.4.6.13 cartesianproduct -->
  <xsl:template match="apply[*[1][self::cartesianproduct or self::vectorproduct]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="2"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          &#215;<!-- times -->
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template
  match="apply[*[1][self::cartesianproduct][count(following-sibling::reals)=count(following-sibling::*)]]"
  priority="2">
    <msup>
      <xsl:apply-templates select="*[2]">
        <xsl:with-param name="p" select="5"/>
      </xsl:apply-templates>
      <mn>
        <xsl:value-of select="count(*)-1"/>
      </mn>
    </msup>
  </xsl:template>


  <!-- 4.4.7.1 sum -->
  <xsl:template match="apply[*[1][self::sum]]">
    <mrow>
      <msubsup>
        <mo>
          &#8721;<!--sum-->
        </mo>
        <mrow>
          <xsl:apply-templates select="lowlimit/*|interval/*[1]|condition/*"/>
        </mrow>
        <mrow>
          <xsl:apply-templates select="uplimit/*|interval/*[2]"/>
        </mrow>
      </msubsup>
      <xsl:apply-templates select="*[last()]"/>
    </mrow>
  </xsl:template>

  <!-- 4.4.7.2 product -->
  <xsl:template match="apply[*[1][self::product]]">
    <mrow>
      <msubsup>
        <mo>
          &#8719;<!--product-->
        </mo>
        <mrow>
          <xsl:apply-templates select="lowlimit/*|interval/*[1]|condition/*"/>
        </mrow>
        <mrow>
          <xsl:apply-templates select="uplimit/*|interval/*[2]"/>
        </mrow>
      </msubsup>
      <xsl:apply-templates select="*[last()]"/>
    </mrow>
  </xsl:template>

  <!-- 4.4.7.3 limit -->
  <xsl:template match="apply[*[1][self::limit]]">
    <mrow>
      <munder>
        <mi>limit</mi>
        <mrow>
          <xsl:apply-templates select="lowlimit|condition/*"/>
        </mrow>
      </munder>
      <xsl:apply-templates select="*[last()]"/>
    </mrow>
  </xsl:template>

  <xsl:template match="apply[limit]/lowlimit" priority="3">
    <mrow>
      <xsl:apply-templates select="../bvar/node()"/>
      <mo>
        &#8594;<!--rightarrow-->
      </mo>
      <xsl:apply-templates/>
    </mrow>
  </xsl:template>


  <!-- 4.4.7.4 tendsto -->
  <xsl:template match="apply[*[1][self::tendsto]]">
    <xsl:param name="p"/>
    <xsl:call-template name="binary">
      <xsl:with-param name="this-p" select="2"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>
          <xsl:choose>
            <xsl:when test="@type='above'">
              &#8600;<!--searrow-->
            </xsl:when>
            <xsl:when test="@type='below'">
              &#8599;<!--nearrow-->
            </xsl:when>
            <xsl:when test="@type='two-sided'">
              &#8594;<!--rightarrow-->
            </xsl:when>
            <xsl:otherwise>
              &#8594;<!--rightarrow-->
            </xsl:otherwise>
          </xsl:choose>
        </mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.8.1 trig -->
  <xsl:template match="apply[*[1][
 self::sin or self::cos or self::tan or self::sec or
 self::csc or self::cot or self::sinh or self::cosh or
 self::tanh or self::sech or self::csch or self::coth or
 self::arcsin or self::arccos or self::arctan or self::arccosh
 or self::arccot or self::arccoth or self::arccsc or
 self::arccsch or self::arcsec or self::arcsech or
 self::arcsinh or self::arctanh or self::ln]]">
    <mrow>
      <mi>
        <xsl:value-of select="local-name(*[1])"/>
      </mi>
      <xsl:apply-templates select="*[2]">
        <xsl:with-param name="p" select="7"/>
      </xsl:apply-templates>
    </mrow>
  </xsl:template>




  <!-- 4.4.8.2 exp -->
  <xsl:template match="apply[*[1][self::exp]]">
    <msup>
      <mi>
        e<!-- exponential e-->
      </mi>
      <mrow>
        <xsl:apply-templates select="*[2]"/>
      </mrow>
    </msup>
  </xsl:template>

  <!-- 4.4.8.3 ln -->
  <!-- with trig -->

  <!-- 4.4.8.4 log -->
  <xsl:template match="apply[*[1][self::log]]">
    <mrow>
      <xsl:choose>
        <xsl:when test="not(logbase) or logbase=10">
          <mi>log</mi>
        </xsl:when>
        <xsl:otherwise>
          <msub>
            <mi>log</mi>
            <mrow>
              <xsl:apply-templates select="logbase/node()"/>
            </mrow>
          </msub>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:apply-templates select="*[last()]">
        <xsl:with-param name="p" select="7"/>
      </xsl:apply-templates>
    </mrow>
  </xsl:template>


  <!-- 4.4.9.1 mean -->
  <xsl:template match="apply[*[1][self::mean]]">
    <mrow>
      <mo>
        &#9001;<!--langle-->
      </mo>
      <xsl:for-each select="*[position()&gt;1]">
        <xsl:apply-templates select="."/>
        <xsl:if test="position() !=last()">
          <mo>,</mo>
        </xsl:if>
      </xsl:for-each>
      <mo>
        &#9002;<!--rangle-->
      </mo>
    </mrow>
  </xsl:template>


  <!-- 4.4.9.2 sdef -->
  <xsl:template match="sdev">
    <mo>
      &#963;<!--sigma-->
    </mo>
  </xsl:template>

  <!-- 4.4.9.3 variance -->
  <xsl:template match="apply[*[1][self::variance]]">
    <msup>
      <mrow>
        <mo>
          &#963;<!--sigma-->
        </mo>
        <mo>(</mo>
        <xsl:apply-templates select="*[2]"/>
        <mo>)</mo>
      </mrow>
      <mn>2</mn>
    </msup>
  </xsl:template>


  <!-- 4.4.9.4 median -->
  <xsl:template match="median">
    <mo>median</mo>
  </xsl:template>


  <!-- 4.4.9.5 mode -->
  <xsl:template match="mode">
    <mo>mode</mo>
  </xsl:template>

  <!-- 4.4.9.5 moment -->
  <xsl:template match="apply[*[1][self::moment]]">
    <mrow>
      <mo>
        &#9001;<!--langle-->
      </mo>
      <msup>
        <xsl:apply-templates select="*[last()]"/>
        <mrow>
          <xsl:apply-templates select="degree/node()"/>
        </mrow>
      </msup>
      <mo>
        &#9002;<!--rangle-->
      </mo>
    </mrow>
  </xsl:template>

  <!-- 4.4.9.5 momentabout -->
  <xsl:template match="momentabout"/>

  <!-- 4.4.10.1 vector  -->
  <xsl:template match="vector">
    <mrow>
      <mo>(</mo>
      <mtable>
        <xsl:for-each select="*">
          <mtr>
            <mtd>
              <xsl:apply-templates select="."/>
            </mtd>
          </mtr>
        </xsl:for-each>
      </mtable>
      <mo>)</mo>
    </mrow>
  </xsl:template>

  <!-- 4.4.10.2 matrix  -->
  <xsl:template match="matrix">
    <mrow>
      <mo>(</mo>
      <mtable>
        <xsl:apply-templates/>
      </mtable>
      <mo>)</mo>
    </mrow>
  </xsl:template>

  <!-- 4.4.10.3 matrixrow  -->
  <xsl:template match="matrixrow">
    <mtr>
      <xsl:for-each select="*">
        <mtd>
          <xsl:apply-templates select="."/>
        </mtd>
      </xsl:for-each>
    </mtr>
  </xsl:template>

  <!-- 4.4.10.4 determinant  -->
  <xsl:template match="apply[*[1][self::determinant]]">
    <mrow>
      <mi>det</mi>
      <xsl:apply-templates select="*[2]">
        <xsl:with-param name="p" select="7"/>
      </xsl:apply-templates>
    </mrow>
  </xsl:template>

  <xsl:template
  match="apply[*[1][self::determinant]][*[2][self::matrix]]" priority="2">
    <mrow>
      <mo>|</mo>
      <mtable>
        <xsl:apply-templates select="matrix/*"/>
      </mtable>
      <mo>|</mo>
    </mrow>
  </xsl:template>

  <!-- 4.4.10.5 transpose -->
  <xsl:template match="apply[*[1][self::transpose]]">
    <msup>
      <xsl:apply-templates select="*[2]">
        <xsl:with-param name="p" select="7"/>
      </xsl:apply-templates>
      <mi>T</mi>
    </msup>
  </xsl:template>

  <!-- 4.4.10.5 selector -->
  <xsl:template match="apply[*[1][self::selector]]">
    <msub>
      <xsl:apply-templates select="*[2]">
        <xsl:with-param name="p" select="7"/>
      </xsl:apply-templates>
      <mrow>
        <xsl:for-each select="*[position()&gt;2]">
          <xsl:apply-templates select="."/>
          <xsl:if test="position() !=last()">
            <mo>,</mo>
          </xsl:if>
        </xsl:for-each>
      </mrow>
    </msub>
  </xsl:template>

  <!-- *** -->
  <!-- 4.4.10.6 vectorproduct see cartesianproduct -->


  <!-- 4.4.10.7 scalarproduct-->
  <xsl:template match="apply[*[1][self::scalarproduct or self::outerproduct]]">
    <xsl:param name="p" select="0"/>
    <xsl:call-template name="infix">
      <xsl:with-param name="this-p" select="2"/>
      <xsl:with-param name="p" select="$p"/>
      <xsl:with-param name="mo">
        <mo>.</mo>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- 4.4.10.8 outerproduct-->

  <!-- 4.4.11.2 semantics -->
  <xsl:template match="semantics">
    <xsl:apply-templates select="*[1]"/>
  </xsl:template>
  <xsl:template match="semantics[annotation-xml/@encoding='MathML-Presentation']">
    <xsl:apply-templates select="annotation-xml[@encoding='MathML-Presentation']/node()"/>
  </xsl:template>

  <!-- 4.4.12.1 integers -->
  <xsl:template match="integers">
    <mi mathvariant="double-struck">Z</mi>
  </xsl:template>

  <!-- 4.4.12.2 reals -->
  <xsl:template match="reals">
    <mi mathvariant="double-struck">R</mi>
  </xsl:template>

  <!-- 4.4.12.3 rationals -->
  <xsl:template match="rationals">
    <mi mathvariant="double-struck">Q</mi>
  </xsl:template>

  <!-- 4.4.12.4 naturalnumbers -->
  <xsl:template match="naturalnumbers">
    <mi mathvariant="double-struck">N</mi>
  </xsl:template>

  <!-- 4.4.12.5 complexes -->
  <xsl:template match="complexes">
    <mi mathvariant="double-struck">C</mi>
  </xsl:template>

  <!-- 4.4.12.6 primes -->
  <xsl:template match="primes">
    <mi mathvariant="double-struck">P</mi>
  </xsl:template>

  <!-- 4.4.12.7 exponentiale -->
  <xsl:template match="exponentiale">
    <mi>
      e<!-- exponential e-->
    </mi>
  </xsl:template>

  <!-- 4.4.12.8 imaginaryi -->
  <xsl:template match="imaginaryi">
    <mi>
      i<!-- imaginary i-->
    </mi>
  </xsl:template>

  <!-- 4.4.12.9 notanumber -->
  <xsl:template match="notanumber">
    <mi>NaN</mi>
  </xsl:template>

  <!-- 4.4.12.10 true -->
  <xsl:template match="true">
    <mi>true</mi>
  </xsl:template>

  <!-- 4.4.12.11 false -->
  <xsl:template match="false">
    <mi>false</mi>
  </xsl:template>

  <!-- 4.4.12.12 emptyset -->
  <xsl:template match="emptyset">
    <mi>
      &#8709;<!-- emptyset -->
    </mi>
  </xsl:template>


  <!-- 4.4.12.13 pi -->
  <xsl:template match="pi">
    <mi>
      &#960;<!-- pi -->
    </mi>
  </xsl:template>

  <!-- 4.4.12.14 eulergamma -->
  <xsl:template match="eulergamma">
    <mi>
      &#947;<!-- gamma -->
    </mi>
  </xsl:template>

  <!-- 4.4.12.15 infinity -->
  <xsl:template match="infinity">
    <mi>
      &#8734;<!-- infinity -->
    </mi>
  </xsl:template>


  <!-- ****************************** -->
  <xsl:template name="infix" >
    <xsl:param name="mo"/>
    <xsl:param name="p" select="0"/>
    <xsl:param name="this-p" select="0"/>
    <mrow>
      <xsl:if test="$this-p &lt; $p">
        <mo>(</mo>
      </xsl:if>
      <xsl:for-each select="*[position()&gt;1]">
        <xsl:if test="position() &gt; 1">
          <xsl:copy-of select="$mo"/>
        </xsl:if>
        <xsl:apply-templates select=".">
          <xsl:with-param name="p" select="$this-p"/>
        </xsl:apply-templates>
      </xsl:for-each>
      <xsl:if test="$this-p &lt; $p">
        <mo>)</mo>
      </xsl:if>
    </mrow>
  </xsl:template>

  <xsl:template name="binary" >
    <xsl:param name="mo"/>
    <xsl:param name="p" select="0"/>
    <xsl:param name="this-p" select="0"/>
    <mrow>
      <xsl:if test="$this-p &lt; $p">
        <mo>(</mo>
      </xsl:if>
      <xsl:apply-templates select="*[2]">
        <xsl:with-param name="p" select="$this-p"/>
      </xsl:apply-templates>
      <xsl:copy-of select="$mo"/>
      <xsl:apply-templates select="*[3]">
        <xsl:with-param name="p" select="$this-p"/>
      </xsl:apply-templates>
      <xsl:if test="$this-p &lt; $p">
        <mo>)</mo>
      </xsl:if>
    </mrow>
  </xsl:template>

  <xsl:template name="set" >
    <xsl:param name="o" select="'{'"/>
    <xsl:param name="c" select="'}'"/>
    <mrow>
      <mo>
        <xsl:value-of select="$o"/>
      </mo>
      <xsl:choose>
        <xsl:when test="condition">
          <mrow>
            <xsl:apply-templates select="bvar/*[not(self::bvar or self::condition)]"/>
          </mrow>
          <mo>|</mo>
          <mrow>
            <xsl:apply-templates select="condition/node()"/>
          </mrow>
        </xsl:when>
        <xsl:otherwise>
          <xsl:for-each select="*">
            <xsl:apply-templates select="."/>
            <xsl:if test="position() !=last()">
              <mo>,</mo>
            </xsl:if>
          </xsl:for-each>
        </xsl:otherwise>
      </xsl:choose>
      <mo>
        <xsl:value-of select="$c"/>
      </mo>
    </mrow>
  </xsl:template>
</xsl:stylesheet>

