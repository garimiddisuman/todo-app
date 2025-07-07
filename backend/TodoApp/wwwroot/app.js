const API = '/api/todos';

const fetchTodos = async () => {
  const res = await fetch(API);
  return res.json();
};

const addTodo = async (title) => {
  const body = { TodoTitle: title };
  const res = await fetch(API, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });
  return res.json();
};

const deleteTodo = async (id) => {
  await fetch(`${API}/${id}`, { method: 'DELETE' });
};

const addTask = async (todoId, title, dueDate) => {
  const body = { Title: title };
  if (dueDate) body.DueDate = dueDate;
  const res = await fetch(`${API}/${todoId}/tasks`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });
  return res.json();
};

const deleteTask = async (todoId, taskId) => {
  await fetch(`${API}/${todoId}/tasks/${taskId}`, { method: 'DELETE' });
};

const toggleTask = async (todoId, taskId) => {
  const res = await fetch(`${API}/${todoId}/tasks/${taskId}/toggle`, { method: 'POST' });
  return res.json();
};

const setTaskDueDate = async (todoId, taskId, dueDate) => {
  const res = await fetch(`${API}/${todoId}/tasks/${taskId}/duedate`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ DueDate: dueDate })
  });
  return res.json();
};

const getTemplate = (id) => document.getElementById(id).content;

const setTodoTitle = (li, todo) => {
  li.querySelector('.todo-title').textContent = todo.todoTitle || todo.TodoTitle;
};

const setTodoDeleteHandler = (li, todoId) => {
  li.querySelector('.todo-delete').onclick = () => handleDeleteTodo(todoId);
};

const setAddTaskForm = (li, todoId) => {
  const addTaskForm = li.querySelector('.add-task-form');
  const taskInput = addTaskForm.querySelector('.task-input');
  const dueInput = addTaskForm.querySelector('.due-input');
  addTaskForm.onsubmit = (e) => handleAddTask(e, todoId, taskInput, dueInput);
};

const fillTasksList = (li, todo) => {
  const tasksUl = li.querySelector('.todo-tasks');
  tasksUl.innerHTML = '';
  (todo.allTasks || todo.AllTasks || []).forEach(task => {
    tasksUl.appendChild(createTaskItem(task, todo.todoId));
  });
};

const createTodoItem = (todo) => {
  const template = getTemplate('todo-item-template');
  const li = template.cloneNode(true).children[0];
  setTodoTitle(li, todo);
  setTodoDeleteHandler(li, todo.todoId);
  setAddTaskForm(li, todo.todoId);
  fillTasksList(li, todo);
  return li;
};

const setTaskTitle = (tli, task, todoId) => {
  const title = tli.querySelector('.task-title');
  title.textContent = task.title;
  title.onclick = (e) => handleToggleTask(e, todoId, task.id);
};

const setTaskDueInput = (tli, task, todoId) => {
  const taskDue = tli.querySelector('.task-due');
  taskDue.value = task.dueDate ? task.dueDate.substring(0, 10) : '';
  taskDue.addEventListener('change', (e) => handleSetTaskDueDate(todoId, task.id, taskDue.value));
};

const setTaskDeleteBtn = (tli, todoId, taskId) => {
  const tdel = tli.querySelector('.task-delete');
  tdel.onmouseover = () => tdel.classList.add('hover');
  tdel.onmouseout = () => tdel.classList.remove('hover');
  tdel.onclick = (e) => handleDeleteTask(e, todoId, taskId);
};

const setTaskItemClass = (tli, task) => {
  tli.className = 'task-item' + (task.isCompleted ? ' completed' : '');
};

const createTaskItem = (task, todoId) => {
  const template = getTemplate('task-item-template');
  const tli = template.cloneNode(true).children[0];
  setTaskItemClass(tli, task);
  setTaskTitle(tli, task, todoId);
  setTaskDueInput(tli, task, todoId);
  setTaskDeleteBtn(tli, todoId, task.id);
  return tli;
};

const renderTodos = (todos) => {
  const list = document.getElementById('todo-list');
  list.innerHTML = '';
  todos.forEach(todo => {
    list.appendChild(createTodoItem(todo));
  });
};

const handleDeleteTodo = async (todoId) => {
  await deleteTodo(todoId);
  loadTodos();
};

const handleAddTask = async (e, todoId, taskInput, dueInput) => {
  e.preventDefault();
  if (!taskInput.value.trim() || !dueInput.value) return;
  await addTask(todoId, taskInput.value.trim(), dueInput.value);
  taskInput.value = '';
  dueInput.value = '';
  loadTodos();
};

const handleToggleTask = async (e, todoId, taskId) => {
  e.stopPropagation();
  await toggleTask(todoId, taskId);
  loadTodos();
};

const handleSetTaskDueDate = async (todoId, taskId, value) => {
  await setTaskDueDate(todoId, taskId, value);
  loadTodos();
};

const handleDeleteTask = async (e, todoId, taskId) => {
  e.stopPropagation();
  await deleteTask(todoId, taskId);
  loadTodos();
};

const handleAddTodo = async (e) => {
  e.preventDefault();
  const title = document.getElementById('todo-title').value.trim();
  if (!title) return;
  await addTodo(title);
  document.getElementById('todo-title').value = '';
  loadTodos();
};

const loadTodos = async () => {
  const todos = await fetchTodos();
  renderTodos(todos);
};

const main = () => {
  loadTodos();
  document.getElementById('add-todo-form').onsubmit = handleAddTodo;
};

window.onload = main;
