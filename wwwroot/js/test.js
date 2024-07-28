$(document).ready(function () {
	myCallback()
});

function cargarCombos() {
	$.ajax({
		url: 'https://localhost:44380/api/users/students/exp/307041/email',
		type: 'GET',
		dataType: 'json',
		success: function (data) {
			console.log("Primera funcion ejecutada");
		},
		error: function (error) {
			console.log("Error al cargar los combos");
		}
	});
}

function cargarProductos() {
	$.ajax({
		url: 'https://localhost:44380/api/users/students/exp/307041/email',
		type: 'GET',
		dataType: 'json',
		success: function (data) {
			console.log("Segunda funcion ejecutada");
		}
		,

		error: function (error) {
			console.log("Error al cargar los combosasdasd");
		}
	}
	);
}

function recargarTablas() {
	console.log("Recargando tablas...");
}

function cargarDatos() {
	console.log("Cargando datos...");
}


function myCallback() {
	return new Promise((resolve, reject) => {
		cargarCombos();
		cargarProductos();
		recargarTablas();
		cargarDatos();

		// You can add additional logic here if needed

		// Resolve the promise when all methods complete
		resolve();
	});
}

// Call your callback
myCallback()
	.then(() => {
		console.log("All methods executed successfully!");
	})
	.catch((error) => {
		console.error("Error executing methods:", error);
	});