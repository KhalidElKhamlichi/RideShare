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
		
		//the names must match exactly the property names in the ride class in our C# code.
		$user = array("Fullname" => $row['Fullname'],
						 "Email" => $row['Email'],
						 "Phonenumber" => $row['Phonenumber'],
						 "Username" => $row['Username'],
						 "BirthDate" => $row['BirthDate'],
						 "Type" => $row['Type'],
						 "Password" => $row['Password'],
						 "ImageUrl" => $row['ImgPath']
						 );
						 
		
	
	
		//Echo out the rides array in JSON format
		echo json_encode($user);
	}  
      

      
   }


?>  

