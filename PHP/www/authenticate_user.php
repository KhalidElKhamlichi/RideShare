<?php  
   $response = array(); 
   if (isset($_POST['username']) && isset($_POST['password'])) {

      $username = $_POST['username']; 
      $password = $_POST['password'];
      // include db connect class 
      require_once __DIR__ . '\database_connect.php';  

      // connecting to db  
      $db = new DATABASE_CONNECT();
      $conn = $db->connect();
      
      $checkStmt = $conn->prepare("SELECT * FROM users WHERE Username=? AND Password=?");
      $checkStmt->bind_param("ss", $username, $password); 
      $checkStmt->execute();
      $res = $checkStmt->get_result();
      

      //check if username and password exists
      if($res->fetch_assoc() == NULL)
      {
         
         $response["message"] = "Invalid username or password.";    
         
      }
      else
      {
         $response["message"] = "Welcome.";  
      }
      echo $response["message"];
      
   }


?>  