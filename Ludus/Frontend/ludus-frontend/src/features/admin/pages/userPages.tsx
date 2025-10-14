import React, { useEffect, useState } from 'react';
import { Table, Button } from 'antd';
import type { UserDTO } from '../../../features/admin/api/adminApi.ts'
import { useNavigate } from 'react-router-dom';
import  {adminApi} from '../../../features/admin/api/adminApi.ts'

const UsersPage: React.FC = () => {
  const [users, setUsers] = useState<UserDTO[]>([])
  const [loading, setLoading] = useState(true)
  const navigate = useNavigate()

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const data = await adminApi.getAllUsers()
        setUsers(data)
      } catch (err) {
        console.error('Error fetching users:', err)
      } finally {
        setLoading(false)
      }
    }

    fetchUsers()
  }, [])
  const handleBackToDashboard = () => {
    navigate('/dashboard')
  }
  return (
    <div style={{ padding: '20px' }}>
      <h2>Registered Users</h2>
      <Table
        dataSource={users}
        columns={[
          { title: 'Email', dataIndex: 'email', key: 'email' }
        ]}
        rowKey={record => record.id}
        loading={loading}
      />
      <div style={{ marginTop: '20px', textAlign: 'center' }}>
        <Button 
          type="primary" 
          onClick={handleBackToDashboard}
          size="large"
        >
          Back to Dashboard
        </Button>
      </div>
    </div>
  )
}

export default UsersPage