using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApi.Data;

namespace TodoApi.Core.Models
{
    public class TaskHandler : IRequestHandler<TaskCreateCommand, TodoTask>,
    IRequestHandler<TaskUpdateCommand, TodoTask>,
    IRequestHandler<TaskDeleteCommand>,
    IRequestHandler<GetAllTasksQuery, GetAllTasksResponse>,
    IRequestHandler<GetTaskByIdQuery, GetTaskByIdResponse>
    {
        private readonly IMediator _mediator;
        private readonly ITodoTaskRepository _repository;
        private readonly ILogger<TaskHandler> _logger;

        public TaskHandler(IMediator mediator, ITodoTaskRepository repository, ILogger<TaskHandler> logger)
        {
            _mediator= mediator;
            _repository = repository;
            _logger = logger;
        }

        public async Task<TodoTask> Handle(TaskCreateCommand request, CancellationToken cancellationToken)
        {
            var task = new TodoTask {
                Id = Nanoid.Nanoid.Generate(size: 10),
                   Content = request.Content,
                   Done = request.Done };

            _repository.CreateTask(task);
            _logger.LogInformation($"TodoTask created {task.ToString()}");
            return await Task.Run( () => { return task; });
        }

        public async Task<TodoTask> Handle(TaskUpdateCommand request, CancellationToken cancellationToken)
        {
            var task = _repository.GetTaskById(request.Id);
            if ( task == null )
                throw new System.ArgumentNullException("Task");

            task.Done = request.Done;
            _repository.UpdateTask(task);
            _logger.LogInformation($"TodoTask updated {task.ToString()}");
            return await Task.Run( () => { return task; });
        }

        public async Task<Unit> Handle(TaskDeleteCommand request, CancellationToken cancellationToken)
        {
            var task = _repository.GetTaskById(request.Id);
            if ( task == null )
                throw new System.ArgumentNullException("Task");
            await Task.Run (() => {_repository.DeleteTask(task); });
            _logger.LogInformation($"TodoTask deleted {task.ToString()}");
            return Unit.Value;
        }

        public async Task<GetAllTasksResponse> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            var tasks = _repository.GetAllTodos().OrderByDescending(t => t.Id);
            return await Task.Run (() => { return new GetAllTasksResponse(tasks); });
        }

        public async Task<GetTaskByIdResponse> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = _repository.GetTaskById(request.Id);
            return await Task.Run(() => { return new GetTaskByIdResponse(task); });
        }
    }
}

