using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class TableListView : VisualElement
    {
        public class Column
        {
            public string name;
            public Func<VisualElement> title;
            public Func<VisualElement> cellTemplate;
            
            //Bind a cell in the specific row, to refresh VisualElement
            public Action<VisualElement, int> refreshCell;
        }
        
        public class Row
        {
            public VisualElement container;
            public List<VisualElement> cells = new List<VisualElement>();
        }
        
        private VisualElement m_TopContainer;
        private VisualElement m_TitleContainer;

        private List<Column> m_Columns = new List<Column>();
        private List<Row> m_Rows = new List<Row>();

        public Action<int> onAddRow;
        public Action<VisualElement, int> onDeleteRow;

        private const int m_RowHeight = 24;
        
        public TableListView()
        {
            style.height = 300;
            style.marginTop = style.marginLeft = style.marginRight = 10;
            style.borderTopWidth = style.borderBottomWidth =
                style.borderLeftWidth = style.borderRightWidth = 1;
            style.borderTopColor = style.borderBottomColor =
                style.borderLeftColor = style.borderRightColor = Color.black;
            style.borderTopLeftRadius = 2;
            style.borderTopRightRadius = 2;
            style.borderBottomLeftRadius = 2;
            style.borderBottomRightRadius = 2;

            m_TopContainer = new VisualElement()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.RowReverse),
                    height = m_RowHeight,
                    borderBottomWidth = 1,
                    borderBottomColor = Color.black,
                }
            };

            m_TitleContainer = new VisualElement()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    height = m_RowHeight,
                    borderBottomWidth = 1,
                    borderBottomColor = Color.black
                }
            };
            Add(m_TopContainer);
            Add(m_TitleContainer);
            
            CreateAddRowButton();
        }

        private void CreateAddRowButton()
        {
            Button addElementButton = new Button(AddRow)
            {
                style =
                {
                    marginLeft = 0,
                    marginRight = 0
                }
            };
            addElementButton.Add(new Label("+"));
            m_TopContainer.Add(addElementButton);
        }
        
        public void AddColumn(Column column)
        {
            m_Columns.Add(column);
            
            m_TitleContainer.Clear();

            foreach (var col in m_Columns)
            {
                var parameterLabel = new Label(col.name)
                {
                    style =
                    {
                        flexGrow = 1,
                        width = 0,
                        paddingLeft = 0,
                        paddingRight = 0,
                        borderRightWidth = 1,
                        borderRightColor = Color.black,
                        unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter)
                    }
                };
                m_TitleContainer.Add(parameterLabel);
            }

            var placeHolder = new VisualElement()
            {
                style =
                {
                    width = 24
                }
            };
            m_TitleContainer.Add(placeHolder);

        }

        public void AddRow()
        {
            var row = new Row();

            var rowContainer = new VisualElement()
            {
                style =
                {
                    height = m_RowHeight,
                    borderBottomWidth = 1,
                    borderBottomColor = Color.black,
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                }
            };
            row.container = rowContainer;
            
            //Prepare row data first, then create row VisualElement
            onAddRow?.Invoke(m_Rows.Count);

            foreach (var column in m_Columns)
            {
                var cell = column.cellTemplate.Invoke();
                column.refreshCell.Invoke(cell, m_Rows.Count);
                row.cells.Add(cell);

                var cellContainer = new VisualElement()
                {
                    style =
                    {
                        flexGrow = 1,
                        width = 0,
                        borderRightWidth = 1,
                        borderRightColor = Color.black,
                        justifyContent = new StyleEnum<Justify>(Justify.Center)
                    }
                };
                cellContainer.Add(cell);
                rowContainer.Add(cellContainer);

            }
            
            var deleteButton = new Button(() => DeleteRow(row))
            {
                style =
                {
                    marginLeft = 0,
                    marginRight = 0
                }
            };
            deleteButton.Add(new Label("×"));
            deleteButton.style.width = 24;
            rowContainer.Add(deleteButton);

            Add(rowContainer);
            m_Rows.Add(row);
        }

        private void DeleteRow(Row row)
        {
            //prepare data first
            onDeleteRow?.Invoke(row.container, m_Rows.IndexOf(row));
            
            Remove(row.container);
            m_Rows.Remove(row);
            
            foreach (var newRow in m_Rows)
            {
                for (int i = 0; i < m_Columns.Count; i++)
                {
                    m_Columns[i].refreshCell?.Invoke(newRow.cells[i], m_Rows.IndexOf(newRow));
                }
            }
        }

        public void RefreshTable()
        {
            for (int col = 0; col < m_Columns.Count; col++)
            {
                for (int row = 0; row < m_Rows.Count; row++)
                {
                    m_Columns[col].refreshCell?.Invoke(m_Rows[row].cells[col], row);
                }
            }
        }

        public void RefreshRow(int row)
        {
            for (int col = 0; col < m_Columns.Count; col++)
            {
                m_Columns[col].refreshCell?.Invoke(m_Rows[row].cells[col], row);
            }
        }
    }
}