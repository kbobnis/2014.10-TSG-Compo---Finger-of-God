<?php

class DTO{
	
	private $modelJson;
	private $userName;
	
	public function addModel($modelJson){
		$this->modelJson = $modelJson;
	}
	
	public function addUserName($userName){
		$this->userName = $userName;
	}
	
	
	public function getDto(){
		return array(
			"model" => $this->modelJson,
			"userName" => $this->userName,
			"requiredVersion" => "1.15",
			"thisVersionHeadlines" => "Better animations, repeating missions, font fixes",
		);
	}
	
}