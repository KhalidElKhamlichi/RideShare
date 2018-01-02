<?php
	require_once __DIR__ . '\database_connect.php';  
	

	// connecting to db  
    $db = new DATABASE_CONNECT();
    $conn = $db->connect();

    //Create query to retrieve all rides
	$stmt = $conn->prepare("SELECT * FROM rides");
    $stmt->execute();
    $res = $stmt->get_result();
    if(isset($_POST['username']))
    {
    	$username = $_POST['username'];
    	if(isset($_POST['my_rides']))
		{
			

			$checkStmt = $conn->prepare("SELECT * FROM users WHERE Username=?");
	      	$checkStmt->bind_param("s", $username); 
	      	$checkStmt->execute();
	     	$res = $checkStmt->get_result();
	      	$row = $res->fetch_assoc();
			$id = $row['id'];

			$stmt = $conn->prepare("SELECT * FROM rides WHERE Driver=?");
			$stmt->bind_param("i", $id); 
		    $stmt->execute();
		    $res = $stmt->get_result();
		}
		else
		{
			if(isset($_POST['joined']))
			{
				$checkStmt = $conn->prepare("SELECT * FROM users WHERE Username=?");
		      	$checkStmt->bind_param("s", $username); 
		      	$checkStmt->execute();
		     	$res = $checkStmt->get_result();
		      	$row = $res->fetch_assoc();
				$id = $row['id'];

				$stmt = $conn->prepare("SELECT * FROM rides WHERE Id IN (SELECT RideId FROM users2rides WHERE UserId=?)");
				$stmt->bind_param("i", $id); 
			    $stmt->execute();
			    $res = $stmt->get_result();
			}
			else
			{
				$checkStmt = $conn->prepare("SELECT * FROM users WHERE Username=?");
		      	$checkStmt->bind_param("s", $username); 
		      	$checkStmt->execute();
		     	$res = $checkStmt->get_result();
		      	$row = $res->fetch_assoc();
				$id = $row['id'];

				$stmt = $conn->prepare("SELECT * FROM rides WHERE (Driver<>? OR Driver IS NULL) AND (Seats>0 OR Seats IS NULL)");
				$stmt->bind_param("i", $id); 
			    $stmt->execute();
			    $res = $stmt->get_result();
			}
			
		}
    }
	
	
	if (!$stmt)
	{
		//Query failed
		echo 'Query failed';
	}	
	else
	{
		$rides = array(); //Create an array to hold all of the rides
		//Query successful, begin putting each ride into an array of rides
		
		while ($row = $res->fetch_assoc()) //While there are still rides
		{
			$nameStmt = $conn->prepare("SELECT Username FROM users WHERE id=?");
			$id = intval($row['Driver']);
		    $nameStmt->bind_param("i", $id); 
		    $nameStmt->execute();
		    $result = $nameStmt->get_result();
		    $Row = $result->fetch_assoc(); 
		    $username = $Row["Username"];
		    $d = $row['Driver'];
		    if(is_null($d))
		    	$username = "empty";

		    $submitterStmt = $conn->prepare("SELECT Username FROM users WHERE id=?");
			$subm_id = intval($row['Submitter']);
		    $submitterStmt->bind_param("i", $subm_id); 
		    $submitterStmt->execute();
		    $subm_result = $submitterStmt->get_result();
		    $subm_Row = $subm_result->fetch_assoc(); 
		    $submitter = $subm_Row["Username"];
		    $s = $row['Submitter'];
		    if(is_null($s))
		    	$submitter = "empty";

		    if(is_null($row['Seats']))
		    	$row['Seats'] = "-1";
			//Create an associative array to hold the current ride
			//the names must match exactly the property names in the ride class in our C# code.
			$ride = array("Id" => $row['Id'],
							"Origin" => $row['Origin'],
							 "Destination" => $row['Destination'],
							 "Date" => $row['RideDate'],
							 "Time" => $row['StartTime'],
							 "Stops" => $row['Stops'],
							 "Smokes" => $row['Smokes'],
							 "Seats" => $row['Seats'],
							 "Backbag" => $row['Backbag'],
							 "Travelbag" => $row['Travelbag'],
							 "Driver" => $username,
							 "Submitter" => $submitter
							 );
							 
			//Add the ride to the rides array
			array_push($rides, $ride);
		}
		
		//Echo out the rides array in JSON format
		echo json_encode($rides);
	}
	
	
?>