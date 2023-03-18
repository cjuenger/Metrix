using Autofac;
using io.juenger.Scrum.GitLab.Demo;

// build Autofac container
var containerBuilder = new ContainerBuilder();
containerBuilder.RegisterModule<DemoModule>();
var container = containerBuilder.Build();

// resolve ISprintDemo
var sprintDemo = container.Resolve<ProductDemo>();
await sprintDemo.ShowProductMetricsAsync();