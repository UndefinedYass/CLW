<?xml version="1.0" encoding="utf-8"?>
<ListWatcher ToolsVersion="2.0" xmlns:h="html-parser" name="FSDM Publications" uri="http://fsdmfes.ac.ma/Publications" referenceFile="C:\TOOLS\fbhd-gui\xml\fsdmPublications.ref.html" defaultInterval="3:30" defaultAction="notification">
  <matcher pattern="&lt;ul class=&quot;contact-details&quot;&gt;(.*?)&lt;div class=&quot;pagination&quot;&gt;" options="Singleline">
    <value>
      <ListParser>
        <Item pattern="&lt;li&gt;.\s*.\s*&lt;div&gt;(.*?)&lt;/li&gt;" options="Singleline">
          <value>
            <h:li>
              <h:div>

                <h:p>
                  <h:span >
                    <h:a href="mi:any">FIXED POINT THEOREMS FOR ψ−CONTRACTIVE MAPPING IN
                        <AttributeValue AttributeName="href">
                           <targetProperty UseAs="Link" property="href" /><targetProperty property="ID" />
                        </AttributeValue>
                        <innerHTML>
                           <targetProperty property="title" UseAs="Title" />
                        </innerHTML>
                    </h:a>
                  </h:span>
                </h:p>

                <h:p style="mi:any" h:index="1"> 2021 | LASMA -               Laboratoire de Sciences Mathématiques et Applications -br-tag-here- Mathématical Analysis
                  <innerHTML>
                    <replacer pattern="\n|( {1,})" replacement=" ">
                      <matcher pattern="(.*)&lt;br&gt;" options="Singleline">
                         <tracer message="PART HERE2 #####" />
                        <group index="1">
                           <targetProperty UseAs="SubTitle" property="date" />
                        </group>
                      </matcher>
                      <matcher pattern="&lt;br&gt;(.*)">
                        <group index="1">
                         <targetProperty UseAs="TextContent" property="TextContent" />
                        </group>
                      </matcher>
                    </replacer>
                  </innerHTML>
                </h:p>

              </h:div>
            </h:li>
          </value>
        </Item>
      </ListParser>
    </value>
  </matcher>
</ListWatcher>