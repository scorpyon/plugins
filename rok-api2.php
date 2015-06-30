<?php
$criteria = $_GET["player"];
$player_steamid = $_GET["steamid"];
$api_key_rok = $_GET["api"];
$api_key_steam = "e2yjpkivvmaihgk3qnb1kpvosp8mc058afk";
$rok_app_id = 344760;
//http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=F432A1507D4765B9FEBD51D74A8226F2&steamids=76561197970685910

/*
Need to accept steamid from reign of kings instead if username
get username from steamapi by steamid
user personaname from steamapi and get votes on reign-of-kings.net
send items to player by steamid
*/
$rok_url = "http://reign-of-kings.net/api/?object=servers&element=voters&key=". $api_key_rok ."&format=xml";
$steam_url ="http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=". $api_key_steam ."&steamids=". $player_steamid ."&format=xml";

$db_username = "arqubus1_scorp";
$db_password = "nadene00";
$db_schema = "arqubus1_rokdata";
$conn = new PDO('mysql:host=localhost;dbname='.$db_schema, $db_username, $db_password);

// Do some cool shit);
 $SQL =  'SELECT max(timesvoted) cnt FROM `data` WHERE steamid ="'. $player_steamid .'" and serverapi = "'.$api_key_rok.'" ';
 $results = $conn->query($SQL)->fetch(PDO::FETCH_ASSOC);
 //	
 
 $steamPersona = getPersonaName(get_data($steam_url),"personaname");
 $voteCount = getVoteCount(get_data($rok_url),"NICKNAME", $steamPersona);
 $maxVoteCount = $results['cnt'];
	 if ($maxVoteCount < $voteCount) {
//		 echo $SQL;
 echo ($maxVoteCount == 0) ? $voteCount : $voteCount - $maxVoteCount;

 $statement = $conn->prepare("INSERT INTO data(steamid, inserted, steampersona, timesvoted, serverapi)
         VALUES('".$player_steamid."',curdate(),'".$steamPersona."','".$voteCount."', '".$api_key_rok."')");
 $statement->execute();
 
	 }else{
 echo 0;
	 }

function get_data($url) {
	$ch = curl_init();
	$timeout = 15;
	curl_setopt($ch, CURLOPT_URL, $url);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
	curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, $timeout);
	$data = curl_exec($ch);
	curl_close($ch);
	return $data;
}

function getVoteCount($inXmlset, $needle, $criteria){ 

        $resource    =    xml_parser_create_ns();//Create an XML parser 
        xml_parse_into_struct($resource, $inXmlset, $outArray);// Parse XML data into an array structure 
        xml_parser_free($resource);//Free an XML parser 

		$voteCount = 0;
		 //print_r($outArray[20]);
		// echo "
		
		// ";
		// && ($outArray[7]['tag']=="month" && $outArray[7]['value'] == date("om"))
        for($i=0;$i<count($outArray);$i++){ 
				if(($outArray[$i]['tag']==strtoupper($needle) && strtolower($outArray[$i]['value'])==strtolower($criteria)))
				{
						//$voteCount ++;
						$ii = $i+2;
						$voteCount = $outArray[$ii]['value'];
				}
				
            } 
        return $voteCount;
        }  
function getPersonaName($inXmlset, $needle){ 

        $resource    =    xml_parser_create();//Create an XML parser 
        xml_parse_into_struct($resource, $inXmlset, $outArray);// Parse XML data into an array structure 
        xml_parser_free($resource);//Free an XML parser 

		$voteCount = 0;
		// echo "
		// ";
		
        for($i=0;$i<count($outArray);$i++){ 
				if(strtoupper($outArray[$i]['tag'])==strtoupper($needle))
				{
						$persona = $outArray[$i]['value'];
				}
				
            } 
        return $persona;
        }  
     
?>