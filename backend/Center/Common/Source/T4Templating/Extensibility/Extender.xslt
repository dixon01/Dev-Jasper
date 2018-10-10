<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:data="http://schemas.gorba.com/Center/EntitySpace">
  <xsl:param name="name"/>
  <xsl:param name="extension"/>
  <xsl:variable name="originalContent" select="/" />
  <xsl:variable name="extensionDocument" select="document($extension)" />
  <xsl:variable name="dataRoot" select="$extensionDocument/data:EntitySpace" />
  <xsl:variable name="originalId" select="generate-id(/)" />
  <xsl:variable name="extensionId" select="generate-id($extensionDocument)" />

  <xsl:variable name="originalPartitions" select="$originalContent/data:EntitySpace/data:Partitions/*" />
  <xsl:variable name="extendedPartitions" select="$extensionDocument/data:EntitySpace/data:Partitions/*" />
  
  <xsl:variable name="originalEntities" select="$originalPartitions/data:Entities/*" />
  <xsl:variable name="extendedEntities" select="$extendedPartitions/data:Entities/*" />
  
  <xsl:variable name="originalProperties" select="$originalEntities/data:Properties/*" />
  <xsl:variable name="extendedProperties" select="$extendedEntities/data:Properties/*" />
  <xsl:variable name="all" select="$extendedEntities | $originalEntities" />
  
  <!--Identity template, 
        provides default behavior that copies all content into the output -->
  <xsl:template match="/">
    <xsl:copy>
      <xsl:apply-templates select="$dataRoot"/>
    </xsl:copy>
  </xsl:template>
  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="data:Partitions">
        <xsl:copy>
          <xsl:for-each select="*">
            <xsl:call-template name="ExtendedPartition"/>
          </xsl:for-each>
          <xsl:for-each select="$originalPartitions">
            <xsl:call-template name="OriginalPartition"/>
          </xsl:for-each>
        </xsl:copy>
  </xsl:template>

  <xsl:template match="data:Entities">
    <xsl:variable name="partitionName" select="../@name"/>
    <xsl:choose>
      <xsl:when test="generate-id(ancestor::node()) = $extensionId">
        <xsl:copy>
          <xsl:for-each select="./*">
            <xsl:call-template name="ExtendedEntity"/>
          </xsl:for-each>
          <xsl:for-each select="$originalPartitions[@name=$partitionName]//data:Entity">
            <xsl:call-template name="OriginalEntity"/>
          </xsl:for-each>
        </xsl:copy>
      </xsl:when>
      <xsl:otherwise>
        <xsl:copy>
          <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="data:Properties">
    <xsl:variable name="entityName" select="../@name"/>
    <xsl:choose>
      <xsl:when test="generate-id(ancestor::node()) = $extensionId">
        <xsl:copy>
          <xsl:for-each select="*">
            <xsl:call-template name="ExtendedProperty"/>
          </xsl:for-each>
          <xsl:for-each select="$originalEntities[@name=$entityName]//data:Property">
            <xsl:call-template name="OriginalProperty"/>
          </xsl:for-each>
        </xsl:copy>
      </xsl:when>
      <xsl:otherwise>
        <xsl:copy>
          <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="ExtendedPartition">
    <xsl:variable name="partitionName" select="@name"/>
    <xsl:variable name="originalPartition" select="$originalPartitions[@name=$partitionName]"/>
    <xsl:choose>
      <xsl:when test="$originalPartition">
        <xsl:call-template name="ExtendedPartitionWithExistingOriginal" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:element name="{local-name()}" namespace="{namespace-uri()}">
          <xsl:apply-templates select="@*|node()"/>
        </xsl:element>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="OriginalPartition">
    <xsl:variable name="partitionName" select="@name"/>
    <xsl:if test="not($extendedPartitions[@name=$partitionName])">
      <xsl:element name="{local-name()}" namespace="{namespace-uri()}">
        <xsl:apply-templates select="@*|node()"/>
      </xsl:element>
    </xsl:if>
  </xsl:template>
  <xsl:template name="ExtendedPartitionWithExistingOriginal">
    <xsl:variable name="partitionName" select="@name"/>
    <xsl:variable name="originalPartition" select="$originalPartitions[@name=$partitionName]"/>
    <xsl:copy>
      <!--<xsl:for-each select="@*">
        <xsl:attribute name="{local-name()}">
          <xsl:value-of select="."/>
        </xsl:attribute>
      </xsl:for-each>-->
      <xsl:apply-templates select="@*|node()"/>
      <!--<xsl:for-each select="$originalPartition/@*[not($extendedPartitions/@*[local-name()=$partitionName])]">
        <xsl:attribute name="{local-name()}">
          <xsl:value-of select="."/>
        </xsl:attribute>
      </xsl:for-each>-->
      <xsl:apply-templates select="$originalPartition/data:Enums" />
    </xsl:copy>
  </xsl:template>
  
  <xsl:template name="ExtendedEntity">
    <xsl:variable name="entityName" select="@name"/>
    <xsl:variable name="originalEntity" select="$originalEntities[@name=$entityName]"/>
    <xsl:choose>
      <xsl:when test="$originalEntity">
        <xsl:call-template name="ExtendedEntityWithExistingOriginal" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:element name="{local-name()}" namespace="{namespace-uri(..)}">
          <xsl:apply-templates select="@*|node()"/>
        </xsl:element>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="OriginalEntity">
    <xsl:variable name="entityName" select="@name"/>
    <xsl:if test="not($extendedEntities[@name=$entityName])">
      <xsl:element name="{local-name()}" namespace="{namespace-uri()}">
        <xsl:apply-templates select="@*|node()"/>
      </xsl:element>
    </xsl:if>
  </xsl:template>
  <xsl:template name="ExtendedEntityWithExistingOriginal">
    <xsl:variable name="entityName" select="@name"/>
    <xsl:variable name="originalEntity" select="$originalEntities[@name=$entityName]"/>
    <xsl:element name="{local-name()}" namespace="{namespace-uri()}">
      <xsl:for-each select="@*">
        <xsl:attribute name="{local-name()}">
          <xsl:value-of select="."/>
        </xsl:attribute>
      </xsl:for-each>
      <xsl:for-each select="$originalEntity/@*[not($extendedEntities/@*[local-name()=$entityName])]">
        <xsl:attribute name="{local-name()}">
          <xsl:value-of select="."/>
        </xsl:attribute>
      </xsl:for-each>
      <xsl:apply-templates select="./*" />
    </xsl:element>
  </xsl:template>

  <!-- property not defined in the -->
  <xsl:template name="ExtendedProperty">
    <xsl:variable name="extendedPropertyName" select="@name"/>
    <xsl:variable name="originalProperty" select="$originalProperties[@name=$extendedPropertyName]"/>
      <xsl:choose>
      <xsl:when test="$originalProperty">
        <xsl:call-template name="ExtendedPropertyWithExistingOriginal" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:element name="{local-name()}" namespace="{namespace-uri()}">
      <xsl:apply-templates select="@*|node()"/>
    </xsl:element>
      </xsl:otherwise>
      </xsl:choose>
  </xsl:template>
  <xsl:template name="ExtendedPropertyWithExistingOriginal">
    <xsl:variable name="propertyName" select="@name"/>
    <xsl:variable name="originalProperty" select="$originalProperties[@name=$propertyName]"/>
    <xsl:element name="{local-name()}" namespace="{namespace-uri()}">
      <xsl:for-each select="@*">
        <xsl:attribute name="{local-name()}">
          <xsl:value-of select="."/>
        </xsl:attribute>
      </xsl:for-each>
      <xsl:for-each select="$originalProperty/@*[not($extendedProperties/@*[local-name()=$propertyName])]">
        <xsl:attribute name="{local-name()}">
          <xsl:value-of select="."/>
        </xsl:attribute>
      </xsl:for-each>
    </xsl:element>
  </xsl:template>
  <xsl:template name="OriginalProperty">
    <xsl:variable name="originalPropertyName" select="@name"/>
      <xsl:if test="not($extendedProperties[@name=$originalPropertyName])">
        <xsl:element name="{local-name()}" namespace="{namespace-uri()}">
          <xsl:apply-templates select="@*|node()"/>
        </xsl:element>
      </xsl:if>
  </xsl:template>
</xsl:stylesheet>