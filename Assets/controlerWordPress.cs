using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Text;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;


using OrbCreationExtensions;





public class controlerWordPress : MonoBehaviour {

    public Text title_text;
    public Text body_text;
    public Text comment_text;
    public Text author_description;
    public Renderer renderer;
    public Renderer authorRenderer;
    private string jsonImageFinal;
    public string blogURL;
    private string testText = "test text";
    private string jsonString;
    private string str_parsed;
    public int offset = 0;
    private string commentString;
    private string authorString;
    private string featuredImage;
    private string featuredImageString;
    private string postID;
    private string jsonLogString = "";
    private string commentLogString = "";
    private string featimageLogString = "";
    private string logMsgs = "";
    private string commentUrl = "";
    private string url;
    private string final_output;
    private bool find_feat_image = false;
    private bool find_comments = false;
    private string authorUrl;
    private bool find_author = false;
    private bool found_author = false;
    private bool found_comments = false;
    private bool found_feat_image = false;
    private string final_comment_output;
    private string final_featimage_output;
    private Hashtable jsonNode = null;
    private object selectedNode = null;
    private string[] path = new string[0];
    private string findKey = "";
    private string findValue = "";
    private mainController device;
    private bool continueBody = true;
    private bool foundBody = false;




    void Start() {
        StartCoroutine(DownloadJsonFile("http://" + blogURL + "/wp-json/wp/v2/posts?order=desc&per_page=1&offset=" + offset)).ToString();
        Renderer renderer = GetComponent<Renderer>();

    }

    public void plusplus()
    {
//        Debug.Log("you got plusplus");
        offset += 1;
        StartCoroutine(DownloadJsonFile("http://" + blogURL + "/wp-json/wp/v2/posts?order=desc&per_page=1&offset=" + offset)).ToString();
    }

    public void minusminus()
    {
 //       Debug.Log("you got minusminus");
        offset -= 1;
        if( offset < 0 )
        {
            offset = 0;
        }
        StartCoroutine(DownloadJsonFile("http://" + blogURL + "/wp-json/wp/v2/posts?order=desc&per_page=1&offset=" + offset)).ToString();
    }


    private IEnumerator DownloadJsonFile(string url)
    {
        jsonString = null;
        if (continueBody = true)
        {
            yield return StartCoroutine(DownloadFile(url, fileContents => jsonString = fileContents));
            Debug.Log(jsonString);
            jsonLogString = TruncateStringForEditor(jsonString);
            find_feat_image = true;
            find_comments = true;
            foundBody = true;
        }
    }

    private IEnumerator DownloadCommentFile(string url)
    {
        commentString = null;
        find_comments = false;
        yield return StartCoroutine(DownloadFile(url, fileContents => commentString = fileContents));
        Debug.Log(commentString);
        found_comments = true;
        commentLogString = TruncateStringForEditor(commentString);
    }

    private IEnumerator DownloadAuthorFile(string url)
    {
        authorString = null;
        yield return StartCoroutine(DownloadFile(url, fileContents => authorString = fileContents));
        Debug.Log(authorString);
        found_author = true;
        find_author = false;
        commentLogString = TruncateStringForEditor(authorString);
    }

    private IEnumerator DownloadImageFile(string url)
    {
        featuredImageString = null;
        find_feat_image = false;
        yield return StartCoroutine(DownloadFile(url, fileContents => featuredImageString = fileContents));
        Debug.Log(featuredImageString);
        found_feat_image = true;
        featimageLogString = TruncateStringForEditor(featuredImageString);

    }


    private IEnumerator DownloadFile(string url, System.Action<string> result)
    {
        AddToLog("Downloading " + url);
        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            AddToLog(www.error);
        }
        else
        {
            AddToLog("Downloaded " + www.bytesDownloaded + " bytes");
        }

        result(www.text);
    }

    private IEnumerator setTexture(string url)
    {
        AddToLog("Downloading " + url);
        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            AddToLog(www.error);
        }
        else
        {
            AddToLog("Downloaded " + www.bytesDownloaded + " bytes");
        }


        renderer.material.mainTexture = www.texture;
        renderer.material.SetColor("_Color", Color.white);
    }

    private IEnumerator setAuthor(string url)
    {
        AddToLog("Author Downloading " + url);
        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            AddToLog(www.error);
        }
        else
        {
            AddToLog("Downloaded " + www.bytesDownloaded + " bytes");
        }


        authorRenderer.material.mainTexture = www.texture;
        authorRenderer.material.SetColor("_Color", Color.white);
    }


    private void AddToLog(string msg)
    {
        Debug.Log(msg + "\n" + DateTime.Now.ToString("yyy/MM/dd hh:mm:ss.fff"));

        // for some reason the Editor will generate errors if the string is too long
        int lenNeeded = msg.Length + 1;
        if (logMsgs.Length + lenNeeded > 4096) logMsgs = logMsgs.Substring(0, 4096 - lenNeeded);

        logMsgs = logMsgs + "\n" + msg;
    }


    private string TruncateStringForEditor(string str)
    {
        // for some reason the Editor will generate errors if the string is too long
        if (str.Length > 4096) str = str.Substring(0, 4000) + "\n .... display truncated ....\n";
        return str;
    }

    private string CreateJsonLogString(Hashtable aNode)
    {
        if (aNode == null) return "";
        string aStr = aNode.JsonString().Replace("\t", "  ");
        return TruncateStringForEditor(aStr);
    }

    private string stripHtml(string str)
    {
        if (str != null)
        {
            str_parsed = Regex.Replace(str, @"<[^>]*>", String.Empty);
            return str_parsed;

        }
        else
        {
            str_parsed = str;
            return str_parsed;
        }

    }

    private void Reset()
    {
        Debug.Log("Resetting");
        jsonLogString = "";
        if (jsonNode != null) jsonLogString = CreateJsonLogString(jsonNode);
        path = new string[0];
        findKey = "";
        findValue = "";
        selectedNode = null;
    }

    // Update is called once per frame
    void Update () {

        if (foundBody == true)
        {
            var jsonData = JSON.Parse(jsonString);
            jsonImageFinal = jsonData[0]["featured_media"];
            title_text.text = jsonData[0]["title"]["rendered"];
            body_text.text = stripHtml(jsonData[0]["content"]["rendered"]);
            postID = jsonData[0]["id"];
            authorUrl = jsonData[0]["_links"]["author"][0]["href"];
            foundBody = false;
            find_author = true;
        }

        if (find_feat_image == true)
        {
            featuredImage = StartCoroutine(DownloadImageFile("http://" + blogURL + "/wp-json/wp/v2/media/" + jsonImageFinal)).ToString();
        }
        if ( find_author == true)
        {
            StartCoroutine(DownloadAuthorFile(authorUrl));
        }


        if (found_feat_image == true)
        {
            var jsonImage = JSON.Parse(featuredImageString);
            Debug.Log(jsonImage["guid"]["rendered"]);
            StartCoroutine(setTexture(jsonImage["guid"]["rendered"]));
            found_feat_image = false;
        }

        if (find_comments == true)
        {
            final_comment_output = StartCoroutine(DownloadCommentFile("http://" + blogURL + "/wp-json/wp/v2/comments?post=" + postID)).ToString();
        }


        if (found_author == true)
        {
            var jsonAuthor = JSON.Parse(authorString);
            //Debug.Log(jsonAuthor);
            StartCoroutine(setAuthor(jsonAuthor["avatar_urls"]["96"]));
            author_description.text = stripHtml(jsonAuthor["description"]);
            found_author = false;
        }
        if (found_comments == true)
        {
            var jsonComments = JSON.Parse(commentString);
            if (jsonComments != null)
            {
                comment_text.text = stripHtml(jsonComments[0]["author_name"] + " : " + jsonComments[0]["content"]["rendered"]);
                found_comments = false;
            }
            
        }

    }
}
