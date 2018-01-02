<?php 
function CategorieAge($age)
{
	if ($age < 12) {
		return "Enfant";
	}
	elseif ($age >= 12 && $age < 20) {
		return "Jeune";
	}
	else
		return "Adulte";
}

echo CategorieAge(22);
?> 