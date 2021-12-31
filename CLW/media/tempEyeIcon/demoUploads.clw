<?xml version="1.0" encoding="utf-8"?>
<ListWatcher ToolsVersion="2.0" xmlns:h="html-parser" name="Uploads.v2" uri="http://fsdmfes.ac.ma/uploads/Docs/Files/" referenceFile="C:\TOOLS\fbhd-gui\xml\uploads-reference.html" defaultInterval="2:35" defaultAction="notification" popupWindowTitle="$0 New Fsdm Uploads" unreadButtonCaption="$0 Uploads">
  <ListParser>
    <Item pattern="&lt;tr&gt;&lt;td .*" options="none">
      <value>
        <replacer pattern="&lt;img.*?&gt;" replacement="">
          <replacer pattern="&amp;nbsp;" replacement="" appendBefore="&lt;table&gt;" appendAfter="&lt;/table&gt;">
          <!--tr>
            <td valign="top"></td>
              <td>
                <a href="2019-05-16-10-38-15_8a569ec6f415ab8cd22ee2dece546e776ee2ab2b.pdf">2019-05-16-10-38-15_..&gt;</a>
              </td>
            <td align="right">2019-05-16 12:38  </td>
            <td align="right">179K</td>
            <td>
            </td>
          </tr-->
                                   <tracer message="PART HERE2 #####" />

          <h:table>
            <h:tr>
            <tracer message="Item matcher " />
              <h:td h:index="1">
                            
                <h:a >
                  <AttributeValue AttributeName="href">
                    <targetProperty property="href" UseAs="Link" appendBefore="http://fsdmfes.ac.ma/uploads/Docs/Files/" />
                    <targetProperty property="title" UseAs="Title" />

                    <targetProperty property="ID" />
                  </AttributeValue>
                </h:a>
              </h:td>

              <h:td h:index="2">
                            
                <innerHTML>
                                                        <targetProperty property="date" UseAs="SubTitle" />

                </innerHTML>


                
              </h:td>
              
            </h:tr>
            </h:table>
          </replacer>
        </replacer>
      </value>
    </Item>
  </ListParser>
</ListWatcher>