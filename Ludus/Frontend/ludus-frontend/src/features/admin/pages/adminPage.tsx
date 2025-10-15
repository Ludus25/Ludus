import React, { useEffect, useState } from 'react';
import { http } from '../../../shared/api/http'; 
import { Table } from 'antd';




const UsersPage: React.FC = () => {
  const [users, setUsers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const token = localStorage.getItem('token');
        const res = await http.get('/api/test/names', {
          headers: { Authorization: `Bearer ${token}` },
        });
        setUsers(res.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, []);

  return (
    <div style={{ padding: '20px' }}>
      <h2>Registered Users</h2>
      <Table
        dataSource={users}
        columns={[{ title: 'Email', dataIndex: 'email', key: 'email' }]}
        rowKey="id"
        loading={loading}
      />
    </div>
  );
};

export default UsersPage;
