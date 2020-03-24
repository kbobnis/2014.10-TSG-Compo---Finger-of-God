<?php


class UpdateDBCommand extends  CConsoleCommand{
	
	public function actionUpdateDB(){
		
		$query = "ALTER TABLE `missions` CHANGE `missionNumber` `missionName` VARCHAR(40) NOT NULL;";
		$command = Yii::app()->db->createCommand($query);
		$b = $command->queryAll();
		
	}
}