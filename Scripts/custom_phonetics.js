function showMoreAlphabets()
{
    var moreButtonsHtml = '<input type="submit" id="convertText3" value="U.K. Royal Navy" onclick="convertTextToUKRoyalNavy()"/> <br /> <input type="submit" id="convertText4" value="U.K. Royal Air Force (1924–1942)" onclick="convertTextToUKRoyalAirForce1924()"/> <br /> <input type="submit" id="convertText5" value="U.K. Royal Air Force (1943–1956)" onclick="convertTextToUKRoyalAirForce1943()"/> <br /> <input type="submit" id="convertText6" value="U.K. Royal Air Force Alternates (1943–1956)" onclick="convertTextToUKRoyalAirForce1943Alts()"/> <br /> <input type="submit" id="convertText7" value="U.S. Joint Army/Navy Phonetic Alphabet (1941–1956)" onclick="convertTextToUSJointArmyNavy()"/>';
    
    document.getElementById('moreButtons').innerHTML = moreButtonsHtml;
}

function convertTextToNato2(txt, rslts, cntnr) {
    console.log("Text: " + txt);
    console.log("Results: " + rslts);
    var textToConvert = document.getElementById(txt).value;
    document.getElementById(rslts).innerHTML = textToNato(textToConvert);
    document.getElementById(cntnr).style.display = 'block'
}

function convertTextToNato()
{
    var textToConvert = document.getElementById('textToConvert').value;
    document.getElementById('conversionResults').innerHTML = textToNato(textToConvert);
}

function convertTextToUKRoyalNavy()
{
    var textToConvert = document.getElementById('textToConvert').value;
    document.getElementById('conversionResults').innerHTML = textToUKRoyalNavy(textToConvert);
}

function convertTextToUKRoyalAirForce1924()
{
    var textToConvert = document.getElementById('textToConvert').value;
    document.getElementById('conversionResults').innerHTML = textToUKRoyalAirForce1924(textToConvert);
}

function convertTextToUKRoyalAirForce1943()
{
    var textToConvert = document.getElementById('textToConvert').value;
    document.getElementById('conversionResults').innerHTML = textToUKRoyalAirForce1943(textToConvert);
}

function convertTextToUKRoyalAirForce1943Alts()
{
    var textToConvert = document.getElementById('textToConvert').value;
    document.getElementById('conversionResults').innerHTML = textToUKRoyalAirForce1943Alts(textToConvert);
}

function convertTextToUSJointArmyNavy()
{
    var textToConvert = document.getElementById('textToConvert').value;
    document.getElementById('conversionResults').innerHTML = textToUSJointArmyNavy(textToConvert);
}

function textToNato(text)
{
    var newline = '<br />';
    var results = '';
    
    text = text.toUpperCase();
    
    for (var i=0; i < text.length; i++)
    {
        switch (text.charAt(i))
        {
            case 'A': results = results + 'alfa '; break;
            case 'B': results = results + 'bravo '; break;
            case 'C': results = results + 'charlie '; break;
            case 'D': results = results + 'delta '; break;
            case 'E': results = results + 'echo '; break;
            case 'F': results = results + 'foxtrot '; break;
            case 'G': results = results + 'golf '; break;
            case 'H': results = results + 'hotel '; break;
            case 'I': results = results + 'india '; break;
            case 'J': results = results + 'juliett '; break;
            case 'K': results = results + 'kilo '; break;
            case 'L': results = results + 'lima '; break;
            case 'M': results = results + 'mike '; break;
            case 'N': results = results + 'november '; break;
            case 'O': results = results + 'oscar '; break;
            case 'P': results = results + 'papa '; break;
            case 'Q': results = results + 'quebec '; break;
            case 'R': results = results + 'romeo '; break;
            case 'S': results = results + 'sierra '; break;
            case 'T': results = results + 'tango '; break;
            case 'U': results = results + 'uniform '; break;
            case 'V': results = results + 'victor '; break;
            case 'W': results = results + 'whiskey '; break;
            case 'X': results = results + 'xray '; break;
            case 'Y': results = results + 'yankee '; break;
            case 'Z': results = results + 'zulu '; break;
            case '-': results = results + '(dash) '; break;
            case ' ': results = results + newline + newline; break;
            default: results = results + text.charAt(i) + ' ';
        }
    }
    
    return results;
}

function textToUKRoyalNavy(text)
{
    var newline = '<br />';
    var results = '';
    
    text = text.toUpperCase();
    
    for (var i=0; i < text.length; i++)
    {
        switch (text.charAt(i))
        {
            case 'A': results = results + 'apples '; break;
            case 'B': results = results + 'butter '; break;
            case 'C': results = results + 'charlie '; break;
            case 'D': results = results + 'duff '; break;
            case 'E': results = results + 'edward '; break;
            case 'F': results = results + 'freddy '; break;
            case 'G': results = results + 'george '; break;
            case 'H': results = results + 'harry '; break;
            case 'I': results = results + 'ink '; break;
            case 'J': results = results + 'johnnie '; break;
            case 'K': results = results + 'king '; break;
            case 'L': results = results + 'london '; break;
            case 'M': results = results + 'monkey '; break;
            case 'N': results = results + 'nuts '; break;
            case 'O': results = results + 'orange '; break;
            case 'P': results = results + 'pudding '; break;
            case 'Q': results = results + 'queenie '; break;
            case 'R': results = results + 'robert '; break;
            case 'S': results = results + 'sugar '; break;
            case 'T': results = results + 'tommy '; break;
            case 'U': results = results + 'uncle '; break;
            case 'V': results = results + 'vinegar '; break;
            case 'W': results = results + 'willie '; break;
            case 'X': results = results + 'xerxes '; break;
            case 'Y': results = results + 'yellow '; break;
            case 'Z': results = results + 'zebra '; break;
            case ' ': results = results + newline + newline; break;
            default: results = results + text.charAt(i) + ' ';
        }
    }
    
    return results;
}

function textToUKRoyalAirForce1924(text)
{
    var newline = '<br />';
    var results = '';
    
    text = text.toUpperCase();
    
    for (var i=0; i < text.length; i++)
    {
        switch (text.charAt(i))
        {
            case 'A': results = results + 'ace '; break;
            case 'B': results = results + 'beer '; break;
            case 'C': results = results + 'charlie '; break;
            case 'D': results = results + 'don '; break;
            case 'E': results = results + 'edward '; break;
            case 'F': results = results + 'freddie '; break;
            case 'G': results = results + 'george '; break;
            case 'H': results = results + 'harry '; break;
            case 'I': results = results + 'ink '; break;
            case 'J': results = results + 'johnnie '; break;
            case 'K': results = results + 'king '; break;
            case 'L': results = results + 'london '; break;
            case 'M': results = results + 'monkey '; break;
            case 'N': results = results + 'nuts '; break;
            case 'O': results = results + 'orange '; break;
            case 'P': results = results + 'pip '; break;
            case 'Q': results = results + 'queen '; break;
            case 'R': results = results + 'robert '; break;
            case 'S': results = results + 'sugar '; break;
            case 'T': results = results + 'toc '; break;
            case 'U': results = results + 'uncle '; break;
            case 'V': results = results + 'vic '; break;
            case 'W': results = results + 'william '; break;
            case 'X': results = results + 'x-ray '; break;
            case 'Y': results = results + 'yorker '; break;
            case 'Z': results = results + 'zebra '; break;
            case ' ': results = results + newline + newline; break;
            default: results = results + text.charAt(i) + ' ';
        }
    }
    
    return results;
}

function textToUKRoyalAirForce1943(text)
{
    var newline = '<br />';
    var results = '';
    
    text = text.toUpperCase();
    
    for (var i=0; i < text.length; i++)
    {
        switch (text.charAt(i))
        {
            case 'A': results = results + 'able '; break;
            case 'B': results = results + 'baker '; break;
            case 'C': results = results + 'charlie '; break;
            case 'D': results = results + 'dog '; break;
            case 'E': results = results + 'easy '; break;
            case 'F': results = results + 'fox '; break;
            case 'G': results = results + 'george '; break;
            case 'H': results = results + 'how '; break;
            case 'I': results = results + 'item '; break;
            case 'J': results = results + 'jig '; break;
            case 'K': results = results + 'king '; break;
            case 'L': results = results + 'love '; break;
            case 'M': results = results + 'mike '; break;
            case 'N': results = results + 'nab '; break;
            case 'O': results = results + 'oboe '; break;
            case 'P': results = results + 'peter '; break;
            case 'Q': results = results + 'queen '; break;
            case 'R': results = results + 'roger '; break;
            case 'S': results = results + 'sugar '; break;
            case 'T': results = results + 'tare '; break;
            case 'U': results = results + 'uncle '; break;
            case 'V': results = results + 'victor '; break;
            case 'W': results = results + 'william '; break;
            case 'X': results = results + 'x-ray '; break;
            case 'Y': results = results + 'yoke '; break;
            case 'Z': results = results + 'zebra '; break;
            case ' ': results = results + newline + newline; break;
            default: results = results + text.charAt(i) + ' ';
        }
    }
    
    return results;
}

function textToUKRoyalAirForce1943Alts(text)
{
    var newline = '<br />';
    var results = '';
    
    text = text.toUpperCase();
    
    for (var i=0; i < text.length; i++)
    {
        switch (text.charAt(i))
        {
            case 'A': results = results + 'affirm '; break;
            case 'B': results = results + 'baker '; break;
            case 'C': results = results + 'charlie '; break;
            case 'D': results = results + 'dog '; break;
            case 'E': results = results + 'easy '; break;
            case 'F': results = results + 'fox '; break;
            case 'G': results = results + 'george '; break;
            case 'H': results = results + 'how '; break;
            case 'I': results = results + 'interrogatory '; break;
            case 'J': results = results + 'johnny '; break;
            case 'K': results = results + 'king '; break;
            case 'L': results = results + 'love '; break;
            case 'M': results = results + 'mike '; break;
            case 'N': results = results + 'negat '; break;
            case 'O': results = results + 'oboe '; break;
            case 'P': results = results + 'prep '; break;
            case 'Q': results = results + 'queen '; break;
            case 'R': results = results + 'roger '; break;
            case 'S': results = results + 'sugar '; break;
            case 'T': results = results + 'tare '; break;
            case 'U': results = results + 'uncle '; break;
            case 'V': results = results + 'victor '; break;
            case 'W': results = results + 'william '; break;
            case 'X': results = results + 'x-ray '; break;
            case 'Y': results = results + 'yoke '; break;
            case 'Z': results = results + 'zebra '; break;
            case ' ': results = results + newline + newline; break;
            default: results = results + text.charAt(i) + ' ';
        }
    }
    
    return results;
}

function textToUSJointArmyNavy(text)
{
    var newline = '<br />';
    var results = '';
    
    text = text.toUpperCase();
    
    for (var i=0; i < text.length; i++)
    {
        switch (text.charAt(i))
        {
            case 'A': results = results + 'able '; break;
            case 'B': results = results + 'baker '; break;
            case 'C': results = results + 'charlie '; break;
            case 'D': results = results + 'dog '; break;
            case 'E': results = results + 'easy '; break;
            case 'F': results = results + 'fox '; break;
            case 'G': results = results + 'george '; break;
            case 'H': results = results + 'how '; break;
            case 'I': results = results + 'item '; break;
            case 'J': results = results + 'jig '; break;
            case 'K': results = results + 'king '; break;
            case 'L': results = results + 'love '; break;
            case 'M': results = results + 'mike '; break;
            case 'N': results = results + 'nan '; break;
            case 'O': results = results + 'oboe '; break;
            case 'P': results = results + 'peter '; break;
            case 'Q': results = results + 'queen '; break;
            case 'R': results = results + 'roger '; break;
            case 'S': results = results + 'sugar '; break;
            case 'T': results = results + 'tare '; break;
            case 'U': results = results + 'uncle '; break;
            case 'V': results = results + 'victor '; break;
            case 'W': results = results + 'william '; break;
            case 'X': results = results + 'x-ray '; break;
            case 'Y': results = results + 'yoke '; break;
            case 'Z': results = results + 'zebra '; break;
            case ' ': results = results + newline + newline; break;
            default: results = results + text.charAt(i) + ' ';
        }
    }
    
    return results;
}