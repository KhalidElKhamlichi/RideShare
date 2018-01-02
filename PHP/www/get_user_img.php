<?php  
   if (isset($_POST['username'])) {

      $username = $_POST['username']; 
      
      // include db connect class 
      require_once __DIR__ . '\database_connect.php';  

      // connecting to db  
      $db = new DATABASE_CONNECT();
      $conn = $db->connect();
      
      $checkStmt = $conn->prepare("SELECT ImgPath FROM users WHERE Username=?");
      $checkStmt->bind_param("s", $username); 
      $checkStmt->execute();
      $res = $checkStmt->get_result();
      $row = $res->fetch_assoc();

      
      
      $path = "UsersImgs/".$row["ImgPath"];

      echo $path;
      
   }


?>  

