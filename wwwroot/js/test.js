//const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyMUBob3RtYWlzbC5jb20iLCJqdGkiOiI5N2Q1MTBmMi01NzJlLTQ3MzUtYmFjMS02NDI0OWEyZmM2N2YiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjhhNGEyMWYyLTEyOTAtNGY4Yy1iMzliLWI4NWY2NGJlNzhjYiIsImV4cCI6MTcyODIxMDY5MywiaXNzIjoic3BvcnR1YXEuY29tIiwiYXVkIjoic3BvcnR1YXEuY29tIn0.6vUPF01KJWkmryWjxNblc4pWhuxwIJJE1I2GPfdAzkE';

//const connection = new signalR.HubConnectionBuilder()
//	.withUrl('https://localhost:44380/hubs/lessons')
//	.configureLogging(signalR.LogLevel.Information)
//	.build();

//// Event handler for 'CourseUpdated'
//connection.on('CourseUpdated', function (course) {
//	console.log('Curso actualizado:', course);
//});

//// Start the connection
//connection.start()
//	.then(function () {
//		console.log('Connection established');
//		// Enviar una señal de que la conexión está activa
//		return connection.invoke('CheckConnection');
//	})
//	.catch(function (err) {
//		console.error('Connection error:', err.toString());
//	});