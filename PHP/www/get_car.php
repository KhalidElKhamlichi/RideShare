<?php  
   if (isset($_POST['username'])) {

      $username = $_POST['username']; 
      
      // include db connect class 
      require_once __DIR__ . '\database_connect.php';  

      // connecting to db  
      $db = new DATABASE_CONNECT();
      $conn = $db->connect();
      
      $checkStmt = $conn->prepare("SELECT * FROM users WHERE Username=?");
      $checkStmt->bind_param("s", $username); 
      $checkStmt->execute();
      $res = $checkStmt->get_result();
      $row = $res->fetch_assoc();

    if ($row["Username"] == '')
	{
		//Query failed
		echo 'Query failed';
	}	
	else
	{		
		$car_id = $row["Car"];
		$car_id = intval($car_id);
		$stmt = $conn->prepare("SELECT * FROM cars WHERE Id=?");
      	$stmt->bind_param("i", $car_id); 
      	$stmt->execute();
      	$res1 = $stmt->get_result();
      	$row1 = $res1->fetch_assoc();
		$car = array("Id" => $row1['Id'],
					 "LicensePlate" => $row1['LicensePlate'],
					 "Type" => $row1['Type'],
					 "Color" => $row1['Color']						 
					 );
						 
		
	
		echo json_encode($car);
	}  
      

      
   }


?>  

