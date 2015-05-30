<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                xmlns:dateFormat="urn:dateFormatType"
                exclude-result-prefixes="msxsl"
>
     <xsl:output method="xml" indent="yes"/>
     <xsl:template match="/">
          <xsl:apply-templates />
     </xsl:template>
     <xsl:template match="[TemplatePath]">
          <xsl:value-of select="dateFormat:[MethodName]([Parameters])"/>
     </xsl:template>
</xsl:stylesheet>
