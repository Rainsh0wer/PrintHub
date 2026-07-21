// This WPF client references the PrintHub.Application layer, whose namespace
// (PrintHub.Application) otherwise shadows the WPF base type `Application`
// (System.Windows.Application) inside the PrintHub.Desktop namespace. The alias
// pins the bare identifier `Application` to the WPF type project-wide.
global using Application = System.Windows.Application;
