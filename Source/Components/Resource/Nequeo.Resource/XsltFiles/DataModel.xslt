<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                exclude-result-prefixes="msxsl"
>
     <xsl:output method="html" indent="yes"/>
     <xsl:template match="/">
          <div id="Context[ClassName]" class="class[ClassName]">
               <table>
                    <thead>
                         <tr>
                              [PropertyHeaderNames]
                         </tr>
                    </thead>
                    <tbody>
                         <xsl:for-each select="Context/Items/[ClassName]">
                              <tr>
                                   [PropertyValueNames]
                              </tr>
                         </xsl:for-each>
                    </tbody>
               </table>
          </div>
     </xsl:template>
</xsl:stylesheet>
