
<?php
	require_once __DIR__ . '\database_connect.php';  
	

	// connecting to db  
    $db = new DATABASE_CONNECT();
    $conn = $db->connect();

	
	//Create query to retrieve all contacts
	$stmt = $conn->prepare("SELECT * FROM users");
    $stmt->execute();
    $res = $stmt->get_result();
    
	
	if (!$stmt)
	{
		//Query failed
		echo 'Query failed';
	}	
	else
	{
		$users = array(); //Create an array to hold all of the rides
		//Query successful, begin putting each ride into an array of rides
		
		while ($row = $res->fetch_assoc()) //While there are still rides
		{
			
			//Create an associative array to hold the current ride
			//the names must match exactly the property names in the ride class in our C# code.
			$user = array("Fullname" => $row['Fullname'],
						 "Email" => $row['Email'],
						 "Phonenumber" => $row['Phonenumber'],
						 "Username" => $row['Username'],
						 "Password" => $row['Password'],
						 "ImageUrl" => $row['ImgPath']
						 );
			//Add the ride to the rides array
			array_push($users, $user);
		}
		
		//Echo out the rides array in JSON format
		echo json_encode($users);
	}
	
?>
