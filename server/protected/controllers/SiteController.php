<?php

/**
 * SiteController is the default controller to handle user requests.
 */
class SiteController extends CController
{
	private $deviceId;
	private $privateKey = "ion32gew9va09)HJ)(N#G()VENIDOSK><P[lp{>:MOF!RFWQ";
	
	public function filters(){
		return array(
			//'dummyTheRequest',
			'loadParameters',
			'checkSignature',
		);
	}
	
	public function filterCheckSignature($filterChain){
		
		$builtString = "";
		$signatureSent = "";
		foreach($_POST as $key => $value){
			if (strlen($builtString) > 0){
				$builtString .="&";
			}
			if ($key != "Signature"){
				$builtString .= $key."=".$value;
			} else {
				$signatureSent = $value;
			}
		}
		$builtString .= "key=".$this->privateKey;
		//.NET converts / to %2f when counting singature. Lets do the same
		$builtString = str_replace("/", "%2f", $builtString);
		//.NET converts \ to %5c when counting singature. Lets do the same
		$builtString = str_replace("\\", "%5c", $builtString);
		
		$signatureBuilt = md5($builtString);
		if ($signatureSent != $signatureBuilt){
			
			Yii::log("buildtString $builtString, signature sent: $signatureSent, signature built: $signatureBuilt, ". print_r($_POST, true));
			throw new CHttpException(403, "Signature mismatch");
		}
		$filterChain->run();
	}
	
	public function filterDummyTheRequest($filterChain){
		$_POST["Name"] = "rzecz";
		$_POST["DeviceId"] = "D";
		$_POST["MissionType"] = "Random";
		
		$_POST['MissionStatus'] = "Success";
    	$_POST["MissionName"] = "map1";
    	$_POST["Interventions"] = 11;
    	$_POST["Time"] = 8888;
    	$_POST["Round"] = 10;
    	$_POST["Signature"] = "whoneedssignatures";
		
		$filterChain->run();
	}

	public function filterLoadParameters($filterChain){
		
		$this->deviceId = Yii::app()->request->getPost("DeviceId");
		$signature = Yii::app()->request->getPost("Signature");
		 
		if ( $this->deviceId == null || $signature == null){
			throw new CHttpException(400, "Wrong parameters");
		}
		$filterChain->run();
	}
	
	
    public function actionLoad(){
    	    	
    	$mapName = "0";
    	$missionType = Yii::app()->request->getPost('MissionType');
		//fontend sends 1 or 0. we assume 0 by default.
    	$toRepeat = (int) Yii::app()->request->getPost('ToRepeat');
    	if ( $missionType == null){
    		throw new CHttpException(400, "Wrong parameters");
    	}
    	 
    	$round = 0;
    	$actualRound = 0;
    	if ($missionType == "Specified"){
    		$query = "SELECT missionName, round FROM missions WHERE deviceId='".$this->deviceId."' AND missionType='$missionType' AND isSuccess='1' ORDER BY round DESC";
    		$command = Yii::app()->db->createCommand($query);
    		$b = $command->queryAll();
    		$command->reset();
			if ($b == null || $b[0] == null){
				$b = array();
				$b[0] = array("missionName" => 0, "round" => 0);
			}
			$actualRound = 0;
			$missionsInRounds = array();
			foreach($b as $key => $arrayValue){
				$tmpRound = $arrayValue["round"]; 
				if ($tmpRound > $actualRound){
					$actualRound = $tmpRound;
				}
				if (!isset($missionsInRounds[$tmpRound])){
					$missionsInRounds[$tmpRound] = array();
				}
				$missionsInRounds[$tmpRound][] = $arrayValue["missionName"];
			}
			
    		$maps = $this->maps();
    		
    		$mapNumber = -1; //we will increase this number, co -1 is start.
    		foreach($maps as $key => $name){
    			if (in_array($name, $missionsInRounds[$actualRound])){
    				$mapNumber = $key;
    			}
    		}
    		//we want the next mission served to the player if not repeating
    		if ($toRepeat == 0){
    			$mapNumber ++;
    		}
    		if ($mapNumber < count($maps)){
    			$mapName = $maps[$mapNumber];
    		}
    		
    		$url = "protected/data/Maps/specified/$mapName.json";
    	} else {
	    	$dir = opendir('protected/data/Maps/random/'); # This is the directory it will count from
		    $max = 0; # Integer starts at 0 before counting
		
		    # While false is not equal to the filedirectory
		    while (false !== ($file = readdir($dir))) { 
		        if (!in_array($file, array('.', '..')) and !is_dir($file)){
		        	$max++;
		        }
		    }
		    $number = rand(1, $max);
		    
		    $mapName = "map".$number;
    		$url = "protected/data/Maps/random/$mapName.json";
    	}
    	
    	//insert if not exists
    	$query = "INSERT INTO names VALUES ( '".$this->deviceId."', DEFAULT) ON DUPLICATE KEY UPDATE deviceId = deviceId; ";
    	$command = Yii::app()->db->createCommand($query);
		$b = $command->execute();
		$command->reset();
    	$query = "SELECT name FROM names WHERE deviceId='".$this->deviceId."';";
    	$command = Yii::app()->db->createCommand($query);
		$b = $command->queryAll();
		$vals = array_values($b[0]);
		$name = $vals[0];
		
		$mapJson = null;
		if (file_exists($url)){
    		$mapJson = file_get_contents($url);
    	}
    	
		header('Content-type: application/json');
    	$res = array(
			"missionType" => $missionType,
    		"name" => $name,
    		"missionName" => $mapName,
    		"round" => $actualRound,
    		"map" => $mapJson,
    		"results" => $this->getResults($mapName, 1, $missionType),
    	);
    	 
    	 echo json_encode( $res); 
    }
    
    public function actionSave(){
    	
    	$missionStatus = Yii::app()->request->getPost('MissionStatus');
    	$missionName = urldecode( Yii::app()->request->getPost("MissionName") );
    	$interventions = Yii::app()->request->getPost("Interventions");
    	$time = Yii::app()->request->getPost("Time");
    	$missionType = Yii::app()->request->getPost('MissionType');
    	$round = Yii::app()->request->getPost('Round');
    	$getScores = Yii::app()->request->getPost('GetScores');
    		
    	if ( $missionName == null || $missionStatus == null || $time == null || $missionType == null || $round == null){
    		throw new CHttpException(400, "Wrong parameters");
    	}
    	$isSuccess = ($missionStatus=='Success')?1:0;
    	$getScores = $getScores?true:false;
    	
    	$query = "INSERT INTO names VALUES ( '".$this->deviceId."', DEFAULT) ON DUPLICATE KEY UPDATE deviceId = deviceId; ";
    	$query .= "INSERT INTO missions VALUES(NULL, '".$this->deviceId."', '$missionType', $round, '$missionName', $isSuccess, CURRENT_TIMESTAMP, $interventions, $time);";
    	
    	$command = Yii::app()->db->createCommand($query);
    	//i can not insert and select in one query. insert need execute, select needs queryAll() methods 
    	$b = $command->execute();
    	$command->reset();
    	
    	header('Content-type: application/json');
    	$results = null; 
    	if ($getScores){
    		$results = $this->getResults($missionName, $isSuccess, $missionType);
    	}
    	echo json_encode( array( "resultsHere" => $getScores,  "results" => $results));
    }
    
    private function getResults($missionName, $isSuccess, $missionType){
    	$query = "SELECT * FROM (SELECT deviceId, (SELECT name FROM names where names.deviceId=missions.deviceId) as name, timestamp, interventions, time FROM missions where missionType = '".$missionType."' and missionName = '$missionName' and isSuccess = '$isSuccess' ORDER BY interventions, time ASC) as missions  GROUP BY interventions, time, deviceId";
    	$command = Yii::app()->db->createCommand($query);
    	$b = $command->queryAll();
    	
    	
    	return $this->sortResultsFirstAndNeighbours($b);
    }
    
    private function sortResultsFirstAndNeighbours($rows){
    	$uniquePlayers = array();
    	$me = null;
    	$bestPlayer = null;
    	
    	$beforeLast = null;
    	$last = null;
    	$afterMe = null;
    	foreach($rows as $row){
    		if (!isset($uniquePlayers[$row["deviceId"]])){
    			
    			$uniquePlayers[$row["deviceId"]] = $row;
    			$uniquePlayers[$row["deviceId"]]["place"] = count($uniquePlayers);
    			
    			$actual = $uniquePlayers[$row["deviceId"]];
    			
    			if (count($uniquePlayers) == 1){
    				$bestPlayer = $actual;
    			}
    			if ($me != null && $afterMe == null){
    				$afterMe = $actual;
    			}
    			if ($row["deviceId"] == $this->deviceId){
    				$me = $actual;
    			} else if ($me == null) {
    				$beforeLast = $last;
    				$last = $actual;
    			}
    		}
    		if ($afterMe != null){
    			break; //little optimization
    		}
    	}

    	//default order
    	// 1. first place
    	// 2. two before me
    	// 3. before me
    	// 4. me 
    	// 5. after me
    	if ($bestPlayer["deviceId"] == $this->deviceId || $bestPlayer["deviceId"] == $beforeLast["deviceId"] || $bestPlayer["deviceId"] == $last["deviceId"]){
    		$bestPlayer = null;
    	}
    	if ($beforeLast["deviceId"] == $this->deviceId){
    		$beforeLast = null;
    	}
    	if ($last["deviceId"] == $this->deviceId){
    		$last = null;
    	}
    	if ($afterMe["deviceId"] == $this->deviceId){
    		$afterMe = null;
    	}
    	
    	$players = array();
    	$players[] = $bestPlayer;
    	$players[] = $beforeLast;
    	$players[] = $last;
    	$players[] = $me;
    	$players[] = $afterMe;
    	
    	return $players;
    }
    
    private function sortResultsYoureLast($rows){
    	$uniquePlayers = array();
    	$me = null;
    	foreach($rows as $row){
    		if (!isset($uniquePlayers[$row["deviceId"]])){
    			$uniquePlayers[$row["deviceId"]] = $row;
    			$uniquePlayers[$row["deviceId"]]["place"] = count($uniquePlayers);
    			if ($row["deviceId"] == $this->deviceId){
    				$me = $uniquePlayers[$row["deviceId"]];
    			}
    		}
    		if ($me != null && count($uniquePlayers) > 4){
    			break; //little optimization
    		}
    	}
    	 
    	$uniquePlayers = array_values($uniquePlayers);
    	$firstPlayers = array();
    	$foundMe = false;
    	for($i=0; $i < 4; $i++){
    		$firstPlayers[] = isset($uniquePlayers[$i])?$uniquePlayers[$i]:null;
    		if (isset($uniquePlayers[$i]) && $uniquePlayers[$i]["deviceId"] == $this->deviceId){
    			$foundMe = true;
    		}
    	}
    	 
    	if ($foundMe){
    		$firstPlayers[] = isset($uniquePlayers[4])?$uniquePlayers[4]:null;
    	} else {
    		$firstPlayers[] = $me;
    	}
    	return $firstPlayers;
    }
    
    public function actionChangeName(){
    	$name = Yii::app()->request->getPost("Name");

    	if ($name == null){
    		throw new CHttpException(400, "WrongParameters");
    	}
    	
    	$query = "INSERT INTO names VALUES ('".$this->deviceId."', '$name') ON DUPLICATE KEY UPDATE name='$name'; ";
    	$command = Yii::app()->db->createCommand($query);
    	$b = $command->execute();
    	$command->reset();
    	
    	header('Content-type: application/json');
    	echo json_encode( true );
    }
    
    public function actionGetResults(){
    	
    	$missionName = Yii::app()->request->getPost("MissionName");
    	$missionStatus = Yii::app()->request->getPost("MissionStatus");
    	$missionType = Yii::app()->request->getPost('MissionType');
    	
    	if ($missionName == null || $missionType == null || $missionStatus == null){
    		throw new CHttpException(400, "WrongParameters");
    	}    	
    	
    	header('Content-type: application/json');
    	echo json_encode( $this->getResults($missionName, ($missionStatus=="Success"?1:0), $missionType));    	
    }
    
    public function actionRestartLevels(){
    	
		//inserting mission 0 with new round number. the next mission will be 1 of round + 1. 
    	$query = "INSERT INTO missions SELECT null, '".$this->deviceId."', 'Specified', round+1, 0, 1, CURRENT_TIMESTAMP, 0, 0 FROM missions WHERE missions.deviceId='".$this->deviceId."' ORDER BY round DESC LIMIT 1;";
    	$command = Yii::app()->db->createCommand($query);
    	$b = $command->execute();
    	
    	header('Content-type: application/json');
    	echo json_encode( $b);
    }
    
    public function actionGetInitialData(){
    	
    	$url = "protected/data/model.xml";
		$modelJson = file_get_contents($url);
		
		$query = "SELECT name FROM names where deviceId='".$this->deviceId."'; ";
		$command = Yii::app()->db->createCommand($query);
		$b = $command->queryAll();
		$userName = $b[0]["name"];
		$command->reset();
    	 
    	$dto = new DTO();
    	$dto->addModel($modelJson);
    	$dto->addUserName($userName);
    	
    	header('Content-type: application/json');
    	echo json_encode( $dto->getDto() );
    }
    
    private function maps(){
    	array();
    	$maps[] = "01.w/01";
    	$maps[] = "01.w/02";
    	$maps[] = "01.w/03";
    	
    	$maps[] = "02.w.gas/01";
    	$maps[] = "02.w.gas/02";
    	$maps[] = "02.w.gas/02a";
    	$maps[] = "02.w.gas/03";
    	$maps[] = "02.w.gas/04";
    	$maps[] = "02.w.gas/05";
    	
    	$maps[] = "03.s/01";
    	$maps[] = "03.s/02";
    	$maps[] = "03.s/03";
    	
    	$maps[] = "04.s.gas/01";
    	$maps[] = "04.s.gas/02";
    	$maps[] = "04.s.gas/03";
    	$maps[] = "04.s.gas/04";
    	$maps[] = "04.s.gas/05";
    	
    	$maps[] = "05.w.s.gas/01";
    	$maps[] = "05.w.s.gas/02";
    	$maps[] = "05.w.s.gas/03";
    	$maps[] = "05.w.s.gas/04";
    	$maps[] = "05.w.s.gas/05";
    	$maps[] = "05.w.s.gas/06";
    	
    	$maps[] = "06.w.water/01";
    	$maps[] = "06.w.water/02";
    	$maps[] = "06.w.water/03";
    	$maps[] = "06.w.water/04";
    	$maps[] = "06.w.water/05";
    	$maps[] = "06.w.water/06";
    	$maps[] = "06.w.water/07";
    	
    	$maps[] = "07.s.water/01";
    	$maps[] = "07.s.water/02";
    	$maps[] = "07.s.water/03";
    	$maps[] = "07.s.water/04";
    	$maps[] = "07.s.water/05";
    	$maps[] = "07.s.water/06";
    	
    	$maps[] = "08.w.s.water/01";
    	$maps[] = "08.w.s.water/02";
    	$maps[] = "08.w.s.water/03";
    	$maps[] = "08.w.s.water/04";
    	$maps[] = "08.w.s.water/05";
    	
    	$maps[] = "09.w.water.gas/01";
    	$maps[] = "09.w.water.gas/02";
    	$maps[] = "09.w.water.gas/03";
    	$maps[] = "09.w.water.gas/04";
    	$maps[] = "09.w.water.gas/05";
    	
    	$maps[] = "10.w.s.water.gas/01";
    	$maps[] = "10.w.s.water.gas/02";
    	$maps[] = "10.w.s.water.gas/03";
    	$maps[] = "10.w.s.water.gas/04";
    	$maps[] = "10.w.s.water.gas/05";
    	
    	$maps[] = "11.w.el/01";
    	$maps[] = "11.w.el/02";
    	$maps[] = "11.w.el/03";
    	$maps[] = "11.w.el/04";
    	$maps[] = "11.w.el/05";
    	
    	$maps[] = "12.s.el/01";
    	$maps[] = "12.s.el/02";
    	
    	$maps[] = "13.w.s.el/01";
    	$maps[] = "13.w.s.el/02";
    	$maps[] = "13.w.s.el/03";
    	
    	$maps[] = "14.w.water.el/01";
    	$maps[] = "14.w.water.el/02";
    	$maps[] = "14.w.water.el/03";
    	$maps[] = "14.w.water.el/04";
    	$maps[] = "14.w.water.el/05";
    	$maps[] = "14.w.water.el/06";
    	
    	$maps[] = "15.w.s.water.el/01";
    	$maps[] = "15.w.s.water.el/02";
    	$maps[] = "15.w.s.water.el/03";
    	$maps[] = "15.w.s.water.el/04";
    	
    	$maps[] = "16.w.gas.el/01";
    	$maps[] = "16.w.gas.el/02";
    	$maps[] = "16.w.gas.el/03";
    	$maps[] = "16.w.gas.el/04";
    	
    	$maps[] = "17.s.gas.el/01";
    	$maps[] = "17.s.gas.el/02";
    	$maps[] = "17.s.gas.el/03";
    	$maps[] = "17.s.gas.el/04";
    	
    	$maps[] = "18.w.s.gas.el/01";
    	$maps[] = "18.w.s.gas.el/02";
    	$maps[] = "18.w.s.gas.el/03";
    	$maps[] = "18.w.s.gas.el/04";
    	
    	$maps[] = "19.w.water.gas.el/01";
    	$maps[] = "19.w.water.gas.el/02";
    	$maps[] = "19.w.water.gas.el/03";
    	$maps[] = "19.w.water.gas.el/04";
    	$maps[] = "19.w.water.gas.el/05";
    	
    	$maps[] = "20.w.s.water.gas.el/01";
    	$maps[] = "20.w.s.water.gas.el/02";
    	$maps[] = "20.w.s.water.gas.el/03";
    	return $maps;
    }
}


